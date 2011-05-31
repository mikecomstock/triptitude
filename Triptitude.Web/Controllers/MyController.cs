using System.Web.Mvc;
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
            return Redirect(Url.MyTrips());
            ViewBag.User = currentUser;
            return View();
        }

        // needs security check
        public ActionResult DefaultTrip(User currentUser, int id)
        {
            Trip trip = new TripsRepo().Find(id);
            new UsersRepo().SetDefaultTrip(currentUser, trip);
            return Redirect(Url.Details(trip));
        }
    }
}
