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
            new WebsiteService().AddWebsite(url);
            Trip trip = new TripsRepo().Find(tripId);
            return Redirect(Url.TripDetails(trip));
        }
    }
}