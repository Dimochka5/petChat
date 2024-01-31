using Microsoft.AspNetCore.Mvc;
using petChat.Database;

namespace petChat
{
    public class AuthorizationController : Controller
    {
        ChatDbContext db = new ChatDbContext();

        public IActionResult Index()
        {
            var idCookie = Request.Cookies["id"];
            if (!string.IsNullOrEmpty(idCookie))
            {
                int idUser;
                if (int.TryParse(idCookie, out idUser))
                {
                    return RedirectToAction("Chat", "Main");
                }
            }
            return View("Authorization");
        }

        public IActionResult Authorization() => View();

        [HttpPost]
        public IActionResult Registration(User user)
        {
            if (user != null)
            {
                var newUser = new User() { IdUser = db.Users.Count() == 0 ? 0 : db.Users.Max(u => u.IdUser) + 1, Nickname=user.Nickname,Email=user.Email,Password=user.Password };
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(1)
                };
                Response.Cookies.Append("id", newUser.IdUser.ToString(), cookieOptions);
                db.Add(newUser);
                db.SaveChanges();
                return RedirectToAction("Chat", "Main");
            }
            else { return View("Authorization"); }
        }
        [HttpPost]
        public IActionResult Login(User user,string remember)
        {
            if (user != null)
            {
                var findUser = db.Users.Where(u => u.Email == user.Email && u.Password == user.Password).First();
                if (findUser != null)
                {
                    if (remember == "on")
                    {
                        CookieOptions cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(1)
                        };
                        Response.Cookies.Append("id", findUser.IdUser.ToString(), cookieOptions);
                    }
                    else
                    {
                        CookieOptions cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(1)
                        };
                        Response.Cookies.Append("id", findUser.IdUser.ToString(), cookieOptions);
                    }
                    return RedirectToAction("Chat", "Main");
                }
                return View("Authorization");
            }
            else
            {
                return View("Authorization");
            }
        }
    }
}
