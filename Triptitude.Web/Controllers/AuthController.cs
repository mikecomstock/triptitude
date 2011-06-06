using System.Web.Mvc;
using System.Web.Security;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Form = new LoginForm();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginForm form)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Form = form;
                return View();
            }

            var user = new AuthService().Authenticate(form);
            if (user != null)
            {
                AuthHelper.SetAuthCookie(user);
                return Redirect(Url.MyAccount());
            }
            else
            {
                ModelState.AddModelError("credentials", "Invalid credentials.");
                ViewBag.Form = form;
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~");
        }
    }
}
