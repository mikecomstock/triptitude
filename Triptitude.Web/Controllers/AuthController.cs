using System.Web.Mvc;
using System.Web.Security;
using Triptitude.Biz.Services;

namespace Triptitude.Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = new AuthService().Authenticate(email, password);

            if (user != null)
            {
                string userName = user.Id + "|" + user.Email;
                FormsAuthentication.SetAuthCookie(userName, true);
                return Redirect("~");
            }
            else
            {
                ModelState.AddModelError("invalid", "Invalid credentials!");
                return RedirectToRoute("Login");
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~");
        }
    }
}
