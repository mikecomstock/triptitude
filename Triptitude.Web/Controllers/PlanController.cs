using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class PlanController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            ViewBag.NumberDays = trip.Itinerary.Max(i => i.EndDay) ?? 1;
            return View();
        }

        // Partial only
        public ActionResult DayDetails(Trip trip, int dayNumber)
        {
            ViewBag.DayNumber = dayNumber;
            ViewBag.Trip = trip;
            ViewBag.DayItinerary = trip.Itinerary.Where(i => dayNumber == i.BeginDay || dayNumber == i.EndDay);
            return PartialView("_DayDetails");
        }

        public ActionResult Settings(int id)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);
            ViewBag.Trip = trip;
            ViewBag.Form = tripRepo.GetSettings(trip);
            return View();
        }

        [HttpPost]
        public ActionResult Settings(int id, TripSettings form)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);
            tripRepo.Save(trip, form);
            return Redirect(Url.PublicDetails(trip));
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
            return Redirect(Url.PlanItinerary(trip));
        }
    }
}
