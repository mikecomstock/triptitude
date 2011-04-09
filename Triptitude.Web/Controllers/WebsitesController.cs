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
            return PartialView("websitedialog");
        }

        [HttpPost]
        public ActionResult Create(WebsiteForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            itineraryItemsRepo.Save(form);
            return Redirect(Url.Details(trip));
        }

        public ActionResult Edit(int itineraryItemId, User currentUser)
        {
            var itineraryItem = itineraryItemsRepo.Find(itineraryItemId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, itineraryItem.Trip);
            if (!userOwnsTrip) return Redirect("/");

            WebsiteForm form = new WebsiteForm
                                   {
                                       BeginDay = itineraryItem.BeginDay,
                                       EndDay = itineraryItem.EndDay,
                                       ItineraryItemId = itineraryItemId,
                                       TripId = itineraryItem.Trip.Id,
                                       Url = itineraryItem.Website.URL
                                   };
            ViewBag.Form = form;
            ViewBag.Action = Url.EditWebsite();
            return PartialView("websitedialog");
        }

        [HttpPost]
        public ActionResult Edit(WebsiteForm form, User currentUser)
        {
            var itineraryItem = itineraryItemsRepo.Find(form.ItineraryItemId.Value);
            var oldTrip = itineraryItem.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) Redirect("/");

            itineraryItemsRepo.Save(form);
            return Redirect(Url.Details(itineraryItem.Trip));
        }

        public ActionResult Delete(int itineraryItemId, User currentUser)
        {
            var itineraryItem = itineraryItemsRepo.Find(itineraryItemId);
            var trip = itineraryItem.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) Redirect("/");

            itineraryItemsRepo.Delete(itineraryItem);
            itineraryItemsRepo.Save();
            return Redirect(Url.Details(trip));
        }
    }
}