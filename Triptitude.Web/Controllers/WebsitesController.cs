using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class WebsitesController : Controller
    {
        private TripsRepo tripsRepo;
        private WebsitesRepo websitesRepo;
        private ItineraryItemsRepo itineraryItemsRepo;

        public WebsitesController()
        {
            tripsRepo = new TripsRepo();
            websitesRepo = new WebsitesRepo();
            itineraryItemsRepo = new ItineraryItemsRepo();
        }

        public ActionResult Details(int id)
        {
            Website website = websitesRepo.Find(id);
            ViewBag.Website = website;
            return View();
        }

        public ActionResult Create(User currentUser, int tripId)
        {
            WebsiteForm form = new WebsiteForm { TripId = tripId };
            ViewBag.Form = form;
            ViewBag.Action = Url.CreateWebsite();
            return PartialView("Dialog");
        }

        [HttpPost]
        public ActionResult Create(WebsiteForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = currentUser.OwnsTrips(trip);
            if (!userOwnsTrip) return Redirect("/");

            itineraryItemsRepo.AddWebsiteToTrip(form, trip);
            return Redirect(Url.Details(trip));
        }
    }
}