using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class MyController : Controller
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

        public ActionResult Trips(User currentUser)
        {
            ViewBag.CurrentUser = currentUser;
            return View();
        }

        public ActionResult Settings(User currentUser)
        {
            var form = usersRepo.GetSettingsForm(currentUser);
            ViewBag.Form = form;
            ViewBag.User = currentUser;
            return View();
        }

        [HttpPost]
        public ActionResult Settings(UserSettingsForm form, User currentUser)
        {
            if (ModelState.IsValid)
            {
                UsersRepo.UserSaveAction userSaveAction;
                usersRepo.Save(form, currentUser, out userSaveAction);

                switch (userSaveAction)
                {
                    case UsersRepo.UserSaveAction.NewUserCreated:
                        {
                            EmailService.SentSignupEmail(currentUser);
                            AuthHelper.SetAuthCookie(currentUser);
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
            ViewBag.User = currentUser;
            return View();
        }

        // CSRF vulnerable here. Shouldn't matter though...
        public ActionResult DefaultTrip(User currentUser, int id)
        {
            Trip trip = new TripsRepo().Find(id);
            if (!currentUser.OwnsTrips(trip)) return Redirect("/");

            usersRepo.SetDefaultTrip(currentUser, trip);
            return Redirect(Url.Details(trip));
        }
    }
}
