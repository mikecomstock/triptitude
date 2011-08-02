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
            var placeMarkers = from a in placeActivities
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

            var transportationActivities = activities.OfType<TransportationActivity>();
            var toMarkers = from a in transportationActivities
                            let infoTitle = string.Format("<strong>{0} from <a href='{1}'>{2}</a> to <a href='{3}'>{4}</a></strong>", a.TransportationType.Name, Url.Details(a.FromCity), a.FromCity.ShortName, Url.Details(a.ToCity), a.ToCity.ShortName)
                            let infoBody = Util.DateTimeRangeString(a.BeginDay, null, a.EndDay, null)
                            let infoHtml = infoTitle + "<br/>" + infoBody
                            select new
                                       {
                                           Name = a.ToCity.ShortName,
                                           a.ToCity.Latitude,
                                           a.ToCity.Longitude,
                                           InfoHtml = infoHtml,
                                           ExtendBounds = false
                                       };
            markers.AddRange(toMarkers);

            var fromMarkers = from a in transportationActivities
                              let infoTitle = string.Format("<strong>{0} from <a href='{1}'>{2}</a> to <a href='{3}'>{4}</a></strong>", a.TransportationType.Name, Url.Details(a.FromCity), a.FromCity.ShortName, Url.Details(a.ToCity), a.ToCity.ShortName)
                              let infoBody = Util.DateTimeRangeString(a.BeginDay, null, a.EndDay, null)
                              let infoHtml = infoTitle + "<br/>" + infoBody
                              select new
                                         {
                                             Name = a.FromCity.ShortName,
                                             a.FromCity.Latitude,
                                             a.FromCity.Longitude,
                                             InfoHtml = infoHtml,
                                             ExtendBounds = false
                                         };
            markers.AddRange(fromMarkers);

            var transLines = from a in transportationActivities
                             select new
                                        {
                                            a.TransportationType.PathType,
                                            From = new
                                            {
                                                a.FromCity.Latitude,
                                                a.FromCity.Longitude
                                            },
                                            To = new
                                            {
                                                a.ToCity.Latitude,
                                                a.ToCity.Longitude
                                            }
                                        };
            polyLines.AddRange(transLines);

            #endregion

            var jsonObject = new { markers, polyLines };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }
    }
}
