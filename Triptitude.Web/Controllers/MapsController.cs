using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

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

            var transportations = trip.Transportations;
            var trans = from t in transportations
                        let infoHtml = t.TransportationType.Name + " from <a href='" + Url.Details(t.FromCity) + "'>" + t.FromCity.ShortName + "</a> to <a href='" + Url.Details(t.ToCity) + "'>" + t.ToCity.ShortName + "</a>"
                        select new
                                   {
                                       From = new
                                       {
                                           Name = t.FromCity.ShortName,
                                           Lat = t.FromCity.Latitude,
                                           Lon = t.FromCity.Longitude,
                                           InfoHtml = infoHtml
                                       },
                                       To = new
                                       {
                                           Name = t.ToCity.ShortName,
                                           Lat = t.ToCity.Latitude,
                                           Lon = t.ToCity.Longitude,
                                           InfoHtml = infoHtml
                                       }
                                   };

            var hotelItineraryItems = trip.Itinerary.Where(i => i.Hotel != null);
            var hotels = from i in hotelItineraryItems
                         let infoHtml = string.Format("Lodging at <a href='{0}'>{1}</a>", Url.Details(i.Hotel), i.Hotel.Name)
                         select new
                                    {
                                        Name = i.Hotel.Name,
                                        Lat = i.Hotel.Latitude,
                                        Lon = i.Hotel.Longitude,
                                        InfoHtml = infoHtml
                                    };

            var jsonObject = new { trans, hotels };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }
    }
}
