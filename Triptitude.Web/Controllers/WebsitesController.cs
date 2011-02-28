using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class WebsitesController : Controller
    {
        public ActionResult Details(int id)
        {
            WebsitesRepo websitesRepo = new WebsitesRepo();
            Website website = websitesRepo.Find(id);
            ViewBag.Website = website;
            return View();
        }

        [HttpPost]
        public ActionResult AddToTrip(int tripId, string url)
        {
            Website website = new WebsiteService().AddWebsite(url);
            Trip trip = new TripsRepo().Find(tripId);

            var itineraryItemsRepo = new ItineraryItemsRepo();
            var itineraryItem = itineraryItemsRepo.AddWebsiteToTrip(website, trip);

            return Redirect(Url.EditItineraryItem(itineraryItem));
        }
    }
}