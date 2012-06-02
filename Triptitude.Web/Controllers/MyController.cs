using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class MyController : TriptitudeController
    {
        private readonly UsersRepo usersRepo;
        public MyController()
        {
            usersRepo = new UsersRepo();
        }
        public ActionResult SidePanel()
        {
            return PartialView("_SidePanel");
        }

        public ActionResult Account()
        {
            return Redirect(Url.MyTrips());
            return View();
        }

        public ActionResult Trips()
        {
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        public ActionResult Settings()
        {
            UserSettingsForm form = new UserSettingsForm
            {
                Name = CurrentUser.Name,
                Email = CurrentUser.Email,
                EmailWhenTripsUpdated = CurrentUser.EmailWhenTripsUpdated
            };

            ViewBag.Form = form;
            ViewBag.User = CurrentUser;
            return View();
        }

        [HttpPost]
        public ActionResult Settings(UserSettingsForm form)
        {
            if (ModelState.IsValid)
            {
                UsersRepo.UserSaveAction userSaveAction;
                usersRepo.Save(form, CurrentUser, out userSaveAction);

                switch (userSaveAction)
                {
                    case UsersRepo.UserSaveAction.NewUserCreated:
                        {
                            usersRepo.SetNewGuidIfNeeded(CurrentUser);
                            EmailService.SentSignupEmail(CurrentUser);
                            EmailService.SendUserSignedUp(CurrentUser);
                            AuthHelper.SetAuthCookie(CurrentUser);
                            return Redirect(Url.MyTrips());
                        }
                    case UsersRepo.UserSaveAction.EmailAlreadyTaken:
                        {
                            ModelState.AddModelError("email", "That address is already taken.");
                            break;
                        }
                    default:
                        {
                            // Show success box...
                            break;
                        }
                }
            }

            ViewBag.Form = form;
            ViewBag.User = CurrentUser;
            return View();
        }

        public ActionResult DefaultTrip(int id)
        {
            var trip = CurrentUser.Trips(CurrentUser).FirstOrDefault(t => t.Id == id);
            CurrentUser.DefaultTrip = trip;
            usersRepo.Save();
            return Redirect(Url.Details(trip));
        }
    }
}
