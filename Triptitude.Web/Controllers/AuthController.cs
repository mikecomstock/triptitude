using System;
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
        [HttpGet]
        public ActionResult Login(Guid? token)
        {
            if (token.HasValue)
            {
                var usersRepo = new UsersRepo();
                User user = usersRepo.FindByToken(token.Value);

                if (user != null && !user.GuidIsExpired)
                {
                    AuthHelper.SetAuthCookie(user);
                    return Redirect(Url.MySettings());
                }
                else
                {
                    ModelState.AddModelError("credentials", "Your login link has expired. Use the 'Forgot Password' link to create a new one.");
                }
            }

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
                Session["email"] = form.Email;
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
