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
            if (trip == null) return HttpNotFound();

            var activities = trip.Activities;

            #region Place Activities

            var placeActivities = activities.OfType<PlaceActivity>();
            var placeMarkers = from a in placeActivities.Where(a => a.ActivityPlaces.Any())
                               let place = a.ActivityPlaces.First().Place
                               let infoTitle = string.Format("<strong><a href='{0}'>{1}</a></strong>", Url.Details(place), place.Name)
                               let infoBody = Util.DateTimeRangeString(a.BeginDay, null, a.EndDay, null)
                               let infoHtml = infoTitle + "<br/>" + infoBody
                               where place.Latitude.HasValue && place.Longitude.HasValue
                               select new
                                          {
                                              place.Name,
                                              place.Latitude,
                                              place.Longitude,
                                              InfoHtml = infoHtml,
                                              ExtendBounds = true
                                          };
            markers.AddRange(placeMarkers);

            #endregion

            #region Transportation Activities

            var transportationDoesExtendBounds = !markers.Any();

            var transportationActivities = activities.OfType<TransportationActivity>();
            var toMarkers = from a in transportationActivities.Where(ta => ta.ActivityPlaces.Any(ap => ap.SortIndex == 1))
                            let place = a.ActivityPlaces.First(ap => ap.SortIndex == 1).Place
                            let infoTitle = string.Format("<strong>{0}</strong>", a.Name)
                            let infoBody = Util.DateTimeRangeString(a.BeginDay, null, a.EndDay, null)
                            let infoHtml = infoTitle + "<br/>" + infoBody
                            select new
                                       {
                                           Name = place.Name,
                                           place.Latitude,
                                           place.Longitude,
                                           InfoHtml = infoHtml,
                                           ExtendBounds = transportationDoesExtendBounds
                                       };
            markers.AddRange(toMarkers);

            var fromMarkers = from a in transportationActivities.Where(ta => ta.ActivityPlaces.Any(ap => ap.SortIndex == 0))
                              let place = a.ActivityPlaces.First(ap => ap.SortIndex == 0).Place
                              let infoTitle = string.Format("<strong>{0}</strong>", a.Name)
                              let infoBody = Util.DateTimeRangeString(a.BeginDay, null, a.EndDay, null)
                              let infoHtml = infoTitle + "<br/>" + infoBody
                              select new
                                         {
                                             Name = place.Name,
                                             place.Latitude,
                                             place.Longitude,
                                             InfoHtml = infoHtml,
                                             ExtendBounds = transportationDoesExtendBounds
                                         };
            markers.AddRange(fromMarkers);

            var transLines = from a in transportationActivities.Where(ta => ta.ActivityPlaces.Any(ap => ap.SortIndex == 0) && ta.ActivityPlaces.Any(ap => ap.SortIndex == 1))
                             let fromPlace = a.ActivityPlaces.First(ap => ap.SortIndex == 0).Place
                             let toPlace = a.ActivityPlaces.First(ap => ap.SortIndex == 1).Place
                             select new
                                        {
                                            PathType = a.TransportationType == null ? "geodesic" : a.TransportationType.PathType,
                                            From = new
                                            {
                                                fromPlace.Latitude,
                                                fromPlace.Longitude
                                            },
                                            To = new
                                            {
                                                toPlace.Latitude,
                                                toPlace.Longitude
                                            }
                                        };
            polyLines.AddRange(transLines);

            #endregion

            var jsonObject = new { markers, polyLines };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }
    }
}
