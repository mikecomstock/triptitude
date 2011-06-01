using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class MyController : Controller
    {
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
            ViewBag.User = currentUser;
            return View();
        }

        public ActionResult Settings(User currentUser)
        {
            var form = new UsersRepo().GetSettingsForm(currentUser);
            ViewBag.Form = form;
            ViewBag.User = currentUser;
            return View();
        }

        [HttpPost]
        public ActionResult Settings(UserSettingsForm form, User currentUser)
        {
            var usersRepo = new UsersRepo();
            usersRepo.Save(form, currentUser);

            if (true /*TODO: if valid*/)
            {
                AuthHelper.SetAuthCookie(currentUser);
                return Redirect(Url.MyTrips());
            }
            else
            {
                ViewBag.Form = form;
                ViewBag.User = currentUser;
                return View();
            }
        }

        //TODO: needs security check
        //TODO: convert this to a post
        public ActionResult DefaultTrip(User currentUser, int id)
        {
            Trip trip = new TripsRepo().Find(id);
            new UsersRepo().SetDefaultTrip(currentUser, trip);
            return Redirect(Url.Details(trip));
        }
    }
}
