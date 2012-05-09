using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class UsersController : TriptitudeController
    {
        private UsersRepo repo;
        public UsersController()
        {
            repo = new UsersRepo();
        }

        public ActionResult Details(int id)
        {
            var user = repo.Find(id);
            ViewBag.User = user;
            return View();
        }

        public ActionResult Current()
        {
            return Json(CurrentUser.Json(CurrentUser), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(UserSettingsForm form)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 403; // forbidden
                var modelError = ModelState.Values.First(v => v.Errors.Any()).Errors.First();
                var message = modelError.ErrorMessage;
                return Json(new { message });
            }

            var existingUser = repo.FindByEmail(form.Email);
            if (existingUser != null)
            {
                Response.StatusCode = 403; // forbidden
                return Json(new { message = "You already have an account. Log in instead." });
            }

            try
            {
                var newUser = repo.Create(form);
                repo.Add(newUser);
                repo.Save();

                AuthHelper.SetAuthCookie(newUser);
                EmailService.SentSignupEmail(newUser);
                EmailService.SendUserSignedUp(newUser);

                return Json(newUser);
            }
            catch
            {
                Response.StatusCode = 500; // error
                return Json(new { message = "Something went wrong!" });
            }
        }
    }
}