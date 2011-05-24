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
            Trip trip = new TripsRepo().Find(id);

            //var transportations = trip.Activities.Where(a => a.TransportationActivity != null).Select(ta => ta.TransportationActivity);
            //var trans = from t in transportations
            //            let infoTitle = string.Format("<h3>{0} from <a href='{1}'>{2}</a> to <a href='{3}'>{4}</a></h3>", t.TransportationType.Name, Url.Details(t.FromCity), t.FromCity.ShortName, Url.Details(t.ToCity), t.ToCity.ShortName)
            //            let infoBody = Util.DateTimeRangeString(t.Activity.BeginDay, null, t.Activity.EndDay, null)
            //            let infoHtml = infoTitle + "<br/>" + infoBody
            //            select new
            //                       {
            //                           PathType = t.TransportationType.TravelMode,
            //                           From = new
            //                           {
            //                               Name = t.FromCity.ShortName,
            //                               Lat = t.FromCity.Latitude,
            //                               Lon = t.FromCity.Longitude,
            //                               InfoHtml = infoHtml
            //                           },
            //                           To = new
            //                           {
            //                               Name = t.ToCity.ShortName,
            //                               Lat = t.ToCity.Latitude,
            //                               Lon = t.ToCity.Longitude,
            //                               InfoHtml = infoHtml
            //                           }
            //                       };

            var hotelItineraryItems = trip.Activities.Where(Activity => Activity.LodgingActivity != null).Select(la => la.LodgingActivity);
            var hotels = from i in hotelItineraryItems
                         let infoTitle = string.Format("<h3>Lodging at <a href='{0}'>{1}</a></h3>", Url.Details(i.Hotel), i.Hotel.Name)
                         let numNights = i.Activity.EndDay - i.Activity.BeginDay
                         let nightsText = numNights == 1 ? "night" : "nights"
                         let infoBody = Util.DateTimeRangeString(i.Activity.BeginDay, i.Activity.BeginTime, i.Activity.EndDay, i.Activity.EndTime) + string.Format(" ({0} {1})", numNights, nightsText)
                         let infoHtml = infoTitle + "<br/>" + infoBody
                         select new
                                    {
                                        Name = i.Hotel.Name,
                                        Lat = i.Hotel.Latitude,
                                        Lon = i.Hotel.Longitude,
                                        InfoHtml = infoHtml
                                    };

            var destinationTagItineraryItems = trip.Activities;//.Where(a=> a. .Itinerary.Where(i => i.DestinationTag != null));
            var destinationTags = from dt in destinationTagItineraryItems
                                  let infoTitle = string.Format("<h3>{0} in <a href='{1}'>{2}</a></h3>", dt.Tag.Name, Url.Details(dt.City), dt.City.ShortName)
                                  let infoBody = Util.DateTimeRangeString(dt.BeginDay, dt.BeginTime, dt.EndDay, dt.EndTime)
                                  let infoHtml = infoTitle + "<br/>" + infoBody

                                  select new
                                             {
                                                 Name = dt.City.ShortName,
                                                 Lat = dt.City.Latitude,
                                                 Lon = dt.City.Longitude,
                                                 InfoHtml = infoHtml
                                             };

            var jsonObject = new { hotels, destinationTags };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }
    }
}
