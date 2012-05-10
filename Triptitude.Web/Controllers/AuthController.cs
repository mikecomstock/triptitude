using System.Web.Mvc;
using System.Web.Security;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Response.StatusCode = 403; // forbidden
                return Json(new { message = "Invalid Credentials!" });
            }

            User user = new UsersRepo().FindByEmailAndPassword(email, password);
            if (user == null)
            {
                Response.StatusCode = 403; // forbidden
                return Json(new { message = "Invalid Credentials!" });
            }

            AuthHelper.SetAuthCookie(user);
            return Json(user.Json(user));

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~");
        }

        public ActionResult ForgotPass()
        {
            ViewBag.Sent = false;
            ViewBag.Form = new ForgotPassForm
                               {
                                   Email = (string)Session["email"]
                               };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPass(ForgotPassForm form)
        {
            if (ModelState.IsValid)
            {
                var usersRepo = new UsersRepo();
                User user = usersRepo.FindByEmail(form.Email);
                usersRepo.SetNewGuidIfNeeded(user);
                EmailService.SendForgotPassEmail(user);
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
