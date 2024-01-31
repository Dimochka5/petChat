using System;
using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Web;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using petChat.Database;
using petChat.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace petChat.signalR
{
    public class ChatHub : Hub
    {
        ChatDbContext db = new ChatDbContext();

        public async Task SendMessage(string message, string name, string chat)
        {
            DateTime now = DateTime.Now;
            if (Int32.TryParse(chat, out int idChat))
            {

                Message newMessage = new Message() { Text = message, IdUser = Int32.Parse(name), IdChat = idChat, Time = now };
                newMessage.IdMessage = db.Messages.Count() == 0 ? 0 : db.Messages.Max(m => m.IdMessage) + 1;
                db.Messages.Add(newMessage);
                db.SaveChanges();
                await Clients.Group(idChat.ToString()).SendAsync("ReceiveMessage", message, db.Users.Where(u => u.IdUser == Convert.ToInt32(name)).First().Nickname, $"{now.Hour}:{now.Minute}", db.Chats.Where(c => c.IdChat == idChat).First().Name, newMessage.IdMessage);
            }
        }

        public async Task ChangeChat(int idChat)
        {
            var messages = db.Messages.Where(c => c.IdChat == idChat).ToList();
            foreach (var message in messages)
            {
                await Clients.Caller.SendAsync("GetChat", message.Text, db.Users.Find(message.IdUser).Nickname, $"{message.Time.Hour}:{message.Time.Minute}", message.IdMessage);
            }
        }

        public void DeleteChat(string idChat)
        {
            if (Int32.TryParse(idChat, out int id))
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;database=ChatDb;uid=root;pwd=tiesta2105"))
                {
                    using (MySqlCommand deleteUsers = new MySqlCommand("DELETE From usersInChat where idchat=@id", connection))
                    {
                        using (MySqlCommand deleteChat = new MySqlCommand("delete from chat where id=@id", connection))
                        {
                            deleteUsers.Parameters.AddWithValue("@id", id);
                            deleteChat.Parameters.AddWithValue("@id", id);
                            connection.Open();
                            deleteUsers.ExecuteNonQuery();
                            deleteChat.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public async Task GetChatInfo(int idChat) {
            var chat = db.Chats.Find(idChat);
            if (chat != null) {
                await Clients.Caller.SendAsync("GetChatInfo", chat.Name, chat.Desription);
            }
        }
        public void GetChatUsers(int idChat)
        {
            try
            {
                using (var connection = new MySqlConnection("server=localhost;database=ChatDb;uid=root;pwd=tiesta2105"))
                {
                    var getChatUsers = new MySqlCommand("select nickname from user", connection);
                    using (MySqlDataReader reader = getChatUsers.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
            public Task JoinChat(int idChat)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, idChat.ToString());
        }

        public Task LeaveChat(string idChat)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, idChat);
        }

        public async Task DeleteMessage(int idMessage) {
            var deleteMessage = db.Messages.Where(m => m.IdMessage == idMessage).First();
            if (deleteMessage != null)
            {
                db.Messages.Remove(deleteMessage);
                db.SaveChanges();
                await Clients.Group(deleteMessage.IdChat.ToString()).SendAsync("DeleteMessage", deleteMessage.IdMessage);
            }
        }

        public async Task EditMessage(string text,int idMessage)
        {
            var message = db.Messages.Where(m => m.IdMessage == idMessage).First();
            message.Text = text;
            db.SaveChanges();
            await Clients.Group(message.IdChat.ToString()).SendAsync("EditMessage",message.Text,message.IdMessage);
        }
    }
}
