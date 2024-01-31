using MySqlConnector;
using petChat.Database;

namespace petChat.Models
{
    public class ChatVM
    {
        private readonly ChatDbContext db=new ChatDbContext();
        public User User { get; set; }  

        public List<User> Users { get; set; }

        public List<Chat> Chats { get; set;}

        public List<Message> Messages { get; set;}

        public List<UsersChat> UsersChats { get; set;}

        public ChatVM(User user)
        {
            User = user;
            Users = db.Users.ToList();
            UsersChats = db.UsersChats.ToList();
            List<int> chatsId = new List<int>();

            foreach (var chat in db.UsersChats)
            {
                if (chat.IdUser == user.IdUser)
                {
                    chatsId.Add(chat.IdChat); // Use IdChat instead of IdUser
                    var findChat = db.Chats.FirstOrDefault(c=>c.IdChat==chat.IdChat);
                    Chats.Add(findChat);
                }
            }
            Messages = db.Messages.ToList();
        }

    }
}
