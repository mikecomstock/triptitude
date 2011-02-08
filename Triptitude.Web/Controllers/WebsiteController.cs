using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class WebsiteController : Controller
    {
        [HttpPost]
        public ActionResult AddToTrip(int tripId, string url)
        {
            Website website = new WebsiteService().AddWebsite(url);
            Trip trip = new TripsRepo().Find(tripId);

            new ItineraryService().AddWebsiteToTrip(website, trip);

            return Redirect(Url.TripDetails(trip));
        }
    }
}