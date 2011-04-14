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
        public ActionResult SidePanel(Trip trip)
        {
            ViewBag.Trip = trip;
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
            ViewBag.Transportations = trip.Transportations.Where(t => t.BeginDay == dayNumber || t.EndDay == dayNumber);
            var itineraryItems = trip.Itinerary.Where(i => dayNumber == i.BeginDay || dayNumber == i.EndDay).OrderBy(i => i.BeginDay);

            ViewBag.Hotels = itineraryItems.Where(i => i.Hotel != null);
            ViewBag.Websites = itineraryItems.Where(i => i.Website != null);
            ViewBag.DestinationTags = itineraryItems.Where(i => i.TagDestination != null);
            ViewBag.Editing = currentUser != null && currentUser.DefaultTrip == trip;
            return PartialView("_DayDetails");
        }

        public ActionResult Settings(int id)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);
            ViewBag.Trip = trip;
            ViewBag.Form = tripRepo.GetSettingsForm(trip);
            return View();
        }

        [HttpPost]
        public ActionResult Settings(int id, TripSettingsForm form)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);
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