using System.Collections.Generic;
using System.Dynamic;
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

            var hotelActivities = trip.Activities.OfType<HotelActivity>();
            var hotelMarkers = from ha in hotelActivities
                               let infoTitle = string.Format("<strong>Lodging at <a href='{0}'>{1}</a></strong>", Url.Details(ha.Hotel), ha.Hotel.Name)
                               let numNights = ha.EndDay - ha.BeginDay
                               let nightsText = numNights == 1 ? "night" : "nights"
                               let infoBody = Util.DateTimeRangeString(ha.BeginDay, ha.BeginTime, ha.EndDay, ha.EndTime) + string.Format(" ({0} {1})", numNights, nightsText)
                               let infoHtml = infoTitle + "<br/>" + infoBody
                               select new
                                          {
                                              ha.Hotel.Name,
                                              ha.Hotel.Latitude,
                                              ha.Hotel.Longitude,
                                              InfoHtml = infoHtml,
                                              ExtendBounds = true
                                          };
            markers.AddRange(hotelMarkers);

            var tagActivities = trip.Activities.OfType<TagActivity>();
            var activityMarkers = from dt in tagActivities
                                  let infoTitle = string.Format("<strong>{0} in <a href='{1}'>{2}</a></strong>", dt.Tag.Name, Url.Details(dt.City), dt.City.ShortName)
                                  let infoBody = Util.DateTimeRangeString(dt.BeginDay, dt.BeginTime, dt.EndDay, dt.EndTime)
                                  let infoHtml = infoTitle + "<br/>" + infoBody
                                  select new
                                  {
                                      Name = dt.City.ShortName,
                                      dt.City.Latitude,
                                      dt.City.Longitude,
                                      InfoHtml = infoHtml,
                                      ExtendBounds = true
                                  };
            markers.AddRange(activityMarkers);

            #region Transportation Activities

            var transportationActivities = trip.Activities.OfType<TransportationActivity>();
            var toMarkers = from t in transportationActivities
                            let infoTitle = string.Format("<strong>{0} from <a href='{1}'>{2}</a> to <a href='{3}'>{4}</a></strong>", t.TransportationType.Name, Url.Details(t.FromCity), t.FromCity.ShortName, Url.Details(t.ToCity), t.ToCity.ShortName)
                            let infoBody = Util.DateTimeRangeString(t.BeginDay, null, t.EndDay, null)
                            let infoHtml = infoTitle + "<br/>" + infoBody
                            select new
                                       {
                                           Name = t.ToCity.ShortName,
                                           t.ToCity.Latitude,
                                           t.ToCity.Longitude,
                                           InfoHtml = infoHtml,
                                           ExtendBounds = false
                                       };
            markers.AddRange(toMarkers);

            var fromMarkers = from t in transportationActivities
                              let infoTitle = string.Format("<strong>{0} from <a href='{1}'>{2}</a> to <a href='{3}'>{4}</a></strong>", t.TransportationType.Name, Url.Details(t.FromCity), t.FromCity.ShortName, Url.Details(t.ToCity), t.ToCity.ShortName)
                              let infoBody = Util.DateTimeRangeString(t.BeginDay, null, t.EndDay, null)
                              let infoHtml = infoTitle + "<br/>" + infoBody
                              select new
                                         {
                                             Name = t.FromCity.ShortName,
                                             t.FromCity.Latitude,
                                             t.FromCity.Longitude,
                                             InfoHtml = infoHtml,
                                             ExtendBounds = false
                                         };
            markers.AddRange(fromMarkers);

            var transLines = from t in transportationActivities
                             select new
                                        {
                                            t.TransportationType.PathType,
                                            From = new
                                            {
                                                t.FromCity.Latitude,
                                                t.FromCity.Longitude
                                            },
                                            To = new
                                            {
                                                t.ToCity.Latitude,
                                                t.ToCity.Longitude
                                            }
                                        };
            polyLines.AddRange(transLines);

            #endregion

            var jsonObject = new { markers, polyLines };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }
    }
}
