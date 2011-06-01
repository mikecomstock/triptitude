using System.Web.Mvc;
using System.Web.Security;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

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
                AuthHelper.SetAuthCookie(user);
                return Redirect(Url.MyAccount());
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
