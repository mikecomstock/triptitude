using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class TripsController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Trips = new TripsRepo().FindAll();
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
    }
}