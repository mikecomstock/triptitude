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

        public ActionResult ForgotPass()
        {
            ViewBag.Sent = false;
            ViewBag.Form = new ForgotPassForm();
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPass(ForgotPassForm form)
        {
            if (ModelState.IsValid)
            {
                //TODO: send

                // next i should generate a new login token for the user, with a timeout?, and then send it to the user in an email.
                ViewBag.Sent = true;
            }
            else
            {
                ViewBag.Sent = false;
            }

            ViewBag.Form = form;
            return View();
        }
    }
}
