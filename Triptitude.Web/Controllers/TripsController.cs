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
        public ActionResult DayDetails(Trip trip, int? dayNumber, User currentUser)
        {
            // For unscheduled activities
            dayNumber = dayNumber == -1 ? null : dayNumber;

            ViewBag.DayNumber = dayNumber;
            ViewBag.Trip = trip;
            ViewBag.Editing = currentUser != null && currentUser.DefaultTrip == trip;
            ViewBag.CurrentUserOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            ViewBag.CurrentUser = currentUser;
            return PartialView("_DayDetails");
        }

        public ActionResult PackingList(int id, User currentUser)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            ViewBag.UserOwnsTrip = userOwnsTrip;
            var packingListItems = trip.PackingListItems.Where(pli => userOwnsTrip || pli.Public);
            ViewBag.PackingListItems = packingListItems;
            ViewBag.Tags = packingListItems.SelectMany(pli => pli.Tags).Distinct().ToList().OrderBy(t => t.NiceName);

            return View();
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
        [ValidateAntiForgeryToken]
        public ActionResult Settings(int id, TripSettingsForm form, User currentUser)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            if (ModelState.IsValid)
            {
                tripRepo.Save(trip, form);
                return Redirect(Url.Details(trip));
            }
            else
            {
                ViewBag.Trip = trip;
                ViewBag.Form = form;
                return View();
            }
        }

        public ActionResult Create(int? to)
        {
            var form = new CreateTripForm();

            if (to.HasValue)
            {
                var placesRepo = new PlacesRepo();
                Place place = placesRepo.Find(to.Value);
                form.ToGoogReference = place.GoogReference;
                form.ToGoogId = place.GoogId;
                form.ToName = place.Name;
            }

            ViewBag.Form = form;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateTripForm form, User currentUser)
        {
            if (ModelState.IsValid)
            {
                Trip trip = new TripsRepo().Save(form, currentUser);
                new UsersRepo().SetDefaultTrip(currentUser, trip);
                return Redirect(Url.Details(trip));
            }
            else
            {
                ViewBag.Form = form;
                return View();
            }
        }
    }
}