using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class MapsController : Controller
    {
        public ActionResult Trip(int id)
        {
            var markers = new List<object>();
            var polyLines = new List<object>();

            Trip trip = new TripsRepo().Find(id);
            var activities = trip.Activities;

            var hotelActivities = activities.OfType<HotelActivity>();
            var hotelMarkers = from a in hotelActivities
                               let infoTitle = string.Format("<strong>Lodging at <a href='{0}'>{1}</a></strong>", Url.Details(a.Hotel), a.Hotel.Name)
                               let numNights = a.EndDay - a.BeginDay
                               let nightsText = numNights == 1 ? "night" : "nights"
                               let infoBody = Util.DateTimeRangeString(a.BeginDay, a.BeginTime, a.EndDay, a.EndTime) + string.Format(" ({0} {1})", numNights, nightsText)
                               let infoHtml = infoTitle + "<br/>" + infoBody
                               select new
                                          {
                                              a.Hotel.Name,
                                              a.Hotel.Latitude,
                                              a.Hotel.Longitude,
                                              InfoHtml = infoHtml,
                                              ExtendBounds = true
                                          };
            markers.AddRange(hotelMarkers);

            var placeActivities = activities.OfType<PlaceActivity>();
            var placeMarkers = from a in placeActivities.Where(a => a.Place != null)
                               let infoHtml = string.Format("<strong><a href='{0}'>{1}</a></strong>", Url.Details(a.Place), a.Place.Name)
                               where a.Place.Latitude.HasValue && a.Place.Longitude.HasValue
                               select new
                                          {
                                              a.Place.Name,
                                              a.Place.Latitude,
                                              a.Place.Longitude,
                                              InfoHtml = infoHtml,
                                              ExtendBounds = true
                                          };
            markers.AddRange(placeMarkers);

            #region Transportation Activities

            var transportationDoesExtendBounds = !markers.Any();

            var transportationActivities = activities.OfType<TransportationActivity>();
            var toMarkers = from a in transportationActivities.Where(ta => ta.ToPlace != null)
                            let infoTitle = string.Format("<strong>{0}</strong>", a.Name)
                            let infoBody = Util.DateTimeRangeString(a.BeginDay, null, a.EndDay, null)
                            let infoHtml = infoTitle + "<br/>" + infoBody
                            select new
                                       {
                                           Name = a.ToPlace.Name,
                                           a.ToPlace.Latitude,
                                           a.ToPlace.Longitude,
                                           InfoHtml = infoHtml,
                                           ExtendBounds = transportationDoesExtendBounds
                                       };
            markers.AddRange(toMarkers);

            var fromMarkers = from a in transportationActivities.Where(ta => ta.FromPlace != null)
                              let infoTitle = string.Format("<strong>{0}</strong>", a.Name)
                              let infoBody = Util.DateTimeRangeString(a.BeginDay, null, a.EndDay, null)
                              let infoHtml = infoTitle + "<br/>" + infoBody
                              select new
                                         {
                                             Name = a.FromPlace.Name,
                                             a.FromPlace.Latitude,
                                             a.FromPlace.Longitude,
                                             InfoHtml = infoHtml,
                                             ExtendBounds = transportationDoesExtendBounds
                                         };
            markers.AddRange(fromMarkers);

            var transLines = from a in transportationActivities.Where(ta => ta.ToPlace != null && ta.FromPlace != null)
                             select new
                                        {
                                            PathType = a.TransportationType == null ? "geodesic" : a.TransportationType.PathType,
                                            From = new
                                            {
                                                a.FromPlace.Latitude,
                                                a.FromPlace.Longitude
                                            },
                                            To = new
                                            {
                                                a.ToPlace.Latitude,
                                                a.ToPlace.Longitude
                                            }
                                        };
            polyLines.AddRange(transLines);

            #endregion

            var jsonObject = new { markers, polyLines };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }
    }
}
