using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class TripsController : Controller
    {
        // partial only
        public ActionResult SidePanel(Trip trip, User currentUser)
        {
            ViewBag.Trip = trip;
            ViewBag.UserOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            return PartialView("_SidePanel");
        }

        // partial only
        public ActionResult _Row(Trip trip)
        {
            ViewBag.Trip = trip;
            return PartialView();
        }

        // partial only
        public ActionResult _Rows(IEnumerable<Trip> trips)
        {
            ViewBag.Trips = trips;
            return PartialView();
        }

        //public ActionResult Index()
        //{
        //    ViewBag.Trips = new TripsRepo().FindAll();
        //    return View();
        //}

        public ActionResult Print(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            return View();
        }

        public ActionResult Map(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            return View();
        }

        public ActionResult Details(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            return View();
        }

        // partial only
        public ActionResult DayDetails(Trip trip, int dayNumber, User currentUser)
        {
            ViewBag.DayNumber = dayNumber;
            ViewBag.Trip = trip;
            ViewBag.Activities = trip.Activities.Where(a => a.BeginDay == dayNumber || a.EndDay == dayNumber);
            ViewBag.Editing = currentUser != null && currentUser.DefaultTrip == trip;
            return PartialView("_DayDetails");
        }

        public ActionResult Settings(int id, User currentUser)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);

            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            ViewBag.Trip = trip;
            ViewBag.Form = tripRepo.GetSettingsForm(trip);
            return View();
        }

        [HttpPost]
        public ActionResult Settings(int id, TripSettingsForm form, User currentUser)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);

            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            tripRepo.Save(trip, form);
            return Redirect(Url.Details(trip));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TripCreate form, User currentUser)
        {
            Trip trip = new TripsRepo().Save(form, currentUser.Id);
            new UsersRepo().SetDefaultTrip(currentUser, trip);
            return Redirect(Url.Details(trip));
        }
    }
}