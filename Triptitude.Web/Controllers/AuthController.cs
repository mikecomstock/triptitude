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
        public ActionResult Login(Guid? token, string returnUrl)
        {
            if (token.HasValue)
            {
                var usersRepo = new UsersRepo();
                User user = usersRepo.FindByToken(token.Value);

                if (user != null && !user.GuidIsExpired)
                {
                    AuthHelper.SetAuthCookie(user);
                    return string.IsNullOrWhiteSpace(returnUrl) ? Redirect(Url.MySettings()) : Redirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("credentials", "Your login link has expired. Use the 'Forgot Password' link to create a new one.");
                }
            }

            ViewBag.Form = new LoginForm { ReturnUrl = returnUrl };
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginForm form)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Form = form;
                return View();
            }

            User user = new UsersRepo().FindByEmailAndPassword(form.Email, form.Password);

            if (user != null)
            {
                AuthHelper.SetAuthCookie(user);
                return string.IsNullOrWhiteSpace(form.ReturnUrl) ? Redirect(Url.MyAccount()) : Redirect(form.ReturnUrl);
            }
            else
            {
                ModelState.AddModelError("credentials", "Invalid credentials.");
                Session["email"] = form.Email;
                ViewBag.Form = form;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login2(string email, string password)
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
