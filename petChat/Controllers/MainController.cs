using Microsoft.AspNetCore.Mvc;
using petChat.Database;
using petChat.Models;

namespace petChat.Controllers
{
    public class MainController : Controller
    {
        ChatDbContext db = new ChatDbContext();

        [HttpGet]
        public IActionResult Chat(int id) {
            var idCookie = Request.Cookies["id"]; ;
            if (!string.IsNullOrEmpty(idCookie))
            {
                int idUser;
                if (int.TryParse(idCookie, out idUser))
                {
                    var chatVM = new ChatVM(db.Users.Find(idUser));
                    return View(chatVM);
                }
            }
            return View(); //TODO:return errror
        }
        [HttpGet]
        public IActionResult ExitAccount()
        {
            return RedirectToAction("Authorization", "Authorization");
        }
        [HttpGet]
        public IActionResult Account()
        {
            var idCookie = Request.Cookies["id"]; ;
            if (!string.IsNullOrEmpty(idCookie))
            {
                int idUser;
                if (int.TryParse(idCookie, out idUser))
                {
                    var user = db.Users.Find(idUser); 
                    return View(user);
                }
            }
            return View(); //TODO:return error; 
        }

        [HttpPost]
        public IActionResult CreateChat(List<int> selectedUsers,string chatName)
        {
            Chat newChat= new Chat() { IdChat = db.Chats.Count() == 0 ? 0 : db.Chats.Max(u => u.IdChat) + 1, Name=chatName,Desription=chatName};
            var idCookie = Request.Cookies["id"];
            foreach (int user in selectedUsers)
            {
                var findUser=db.Users.Find(user);
                if (findUser != null)
                {
                    UsersChat usersChat = new UsersChat() { IdUsersChat = db.UsersChats.Count() == 0 ? 0 : db.UsersChats.Max(u => u.IdChat) + 1, IdChat = newChat.IdChat, IdUser = findUser.IdUser };
                    db.UsersChats.Add(usersChat);
                    db.SaveChanges();
                }
            }

            if (!string.IsNullOrEmpty(idCookie))
            {
                int idUser;
                if (int.TryParse(idCookie, out idUser))
                {
                    var User = db.Users.Find(idUser);
                    if (User != null)
                    {
                        var chatVM = new ChatVM(User);
                        UsersChat usersChat = new UsersChat() { IdUsersChat = db.UsersChats.Count() == 0 ? 0 : db.UsersChats.Max(u => u.IdChat) + 1, IdChat = newChat.IdChat, IdUser = User.IdUser };
                        db.Chats.Add(newChat);
                        db.SaveChanges();
                        return View("Chat",chatVM);
                    }
                    return NotFound();
                }
            }
            return NotFound();
        }
    }
}
