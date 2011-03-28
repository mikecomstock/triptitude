using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class MapsController : Controller
    {
        public ActionResult Hotel(int id)
        {
            HotelsRepo hotelsRepo = new HotelsRepo();
            Hotel hotel = hotelsRepo.Find(id);
            ViewBag.Hotel = hotel;

            Random r = new Random();
            IQueryable<Hotel> nearbyHotels = hotelsRepo.FindAll().OrderBy(h => h.Name).Skip(r.Next(0, 100)).Take(10);
            ViewBag.NearbyHotels = nearbyHotels;

            // check out http://stackoverflow.com/questions/1184921/how-to-override-target-blank-in-kml-popups-in-embedded-google-map

            return PartialView();
        }

        public ActionResult Trip(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            return PartialView();
        }
    }
}
