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

            var transportations = trip.Activities.OfType<TransportationActivity>();
            var trans = from t in transportations
                        let infoTitle = string.Format("<strong>{0} from <a href='{1}'>{2}</a> to <a href='{3}'>{4}</a></strong>", t.TransportationType.Name, Url.Details(t.FromCity), t.FromCity.ShortName, Url.Details(t.ToCity), t.ToCity.ShortName)
                        let infoBody = Util.DateTimeRangeString(t.BeginDay, null, t.EndDay, null)
                        let infoHtml = infoTitle + "<br/>" + infoBody
                        select new
                                   {
                                       PathType = t.TransportationType.TravelMode,
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

            var hotelItineraryItems = trip.Activities.OfType<HotelActivity>();
            var hotels = from i in hotelItineraryItems
                         let infoTitle = string.Format("<strong>Lodging at <a href='{0}'>{1}</a></strong>", Url.Details(i.Hotel), i.Hotel.Name)
                         let numNights = i.EndDay - i.BeginDay
                         let nightsText = numNights == 1 ? "night" : "nights"
                         let infoBody = Util.DateTimeRangeString(i.BeginDay, i.BeginTime, i.EndDay, i.EndTime) + string.Format(" ({0} {1})", numNights, nightsText)
                         let infoHtml = infoTitle + "<br/>" + infoBody
                         select new
                                    {
                                        Name = i.Hotel.Name,
                                        Lat = i.Hotel.Latitude,
                                        Lon = i.Hotel.Longitude,
                                        InfoHtml = infoHtml
                                    };

            var destinationTagItineraryItems = trip.Activities.OfType<TagActivity>();
            var destinationTags = from dt in destinationTagItineraryItems
                                  let infoTitle = string.Format("<strong>{0} in <a href='{1}'>{2}</a></strong>", dt.Tag.Name, Url.Details(dt.City), dt.City.ShortName)
                                  let infoBody = Util.DateTimeRangeString(dt.BeginDay, dt.BeginTime, dt.EndDay, dt.EndTime)
                                  let infoHtml = infoTitle + "<br/>" + infoBody

                                  select new
                                             {
                                                 Name = dt.City.ShortName,
                                                 Lat = dt.City.Latitude,
                                                 Lon = dt.City.Longitude,
                                                 InfoHtml = infoHtml
                                             };

            var jsonObject = new { trans, hotels, destinationTags };

            return Json(jsonObject, JsonRequestBehavior.AllowGet);
        }
    }
}
