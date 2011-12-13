using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly TripsRepo tripsRepo;
        private readonly ActivitiesRepo activitiesRepo;
        private readonly TransportationTypesRepo transportationTypesRepo;

        public ActivitiesController()
        {
            tripsRepo = new TripsRepo();
            activitiesRepo = new ActivitiesRepo();
            transportationTypesRepo = new TransportationTypesRepo();
        }

        public ActionResult Delete(int id, User currentUser)
        {
            var activity = activitiesRepo.Find(id);
            var trip = activity.Trip;
            if (!currentUser.OwnsTrips(trip)) return Redirect("/");

            activitiesRepo.Delete(activity);
            activitiesRepo.Save();
            return Redirect(Url.Details(trip));
        }

        public ActionResult Edit(int id, User currentUser, ActivityForm.Tabs selectedTab = ActivityForm.Tabs.Details)
        {
            var activity = activitiesRepo.Find(id);
            var trip = activity.Trip;
            if (!currentUser.OwnsTrips(trip)) return Redirect("/");

            if (activity is TransportationActivity) return EditTransportation(activity as TransportationActivity, selectedTab);
            if (activity is PlaceActivity) return EditPlace(activity as PlaceActivity, selectedTab);

            throw new Exception("Activity type not supported");
        }

        public ActionResult Create(string type, User currentUser, int? placeId)
        {
            switch (type)
            {
                case "transportation": return AddTransportation(currentUser);
                case "place": return AddPlace(placeId, currentUser);
            }

            throw new Exception("Activity type not supported");
        }

        #region Transportation

        private ActionResult AddTransportation(User currentUser)
        {
            var fly = transportationTypesRepo.FindAll().First(tt => tt.Name == "Fly");
            TransportationActivityForm form = new TransportationActivityForm { TripId = currentUser.DefaultTrip.Id, TransportationTypeId = fly.Id };
            ViewBag.Form = form;
            ViewBag.TransportationTypes = transportationTypesRepo.FindAll().OrderBy(t => t.Name);
            ViewBag.Action = Url.ItineraryAddTransportation();
            return PartialView("TransportationDialog");
        }

        [HttpPost]
        public ActionResult AddTransportation(TransportationActivityForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            if (!currentUser.OwnsTrips(trip)) return Redirect("/");

            activitiesRepo.Save(form, currentUser);
            var response = new { status = "OK" };
            return Json(response);
        }

        private ActionResult EditTransportation(TransportationActivity activity, ActivityForm.Tabs selectedTab)
        {
            TransportationActivityForm form = new TransportationActivityForm();
            form.SetBaseProps(activity);
            form.TransportationTypeId = activity.TransportationType == null ? (int?)null : activity.TransportationType.Id;
            form.SelectedTab = selectedTab;

            if (activity.FromPlace != null)
            {
                form.FromName = activity.FromPlace.Name;
                form.FromGoogReference = activity.FromPlace.GoogReference;
                form.FromGoogId = activity.FromPlace.GoogId;
            }
            if (activity.ToPlace != null)
            {
                form.ToName = activity.ToPlace.Name;
                form.ToGoogReference = activity.ToPlace.GoogReference;
                form.ToGoogId = activity.ToPlace.GoogId;
            }

            ViewBag.Form = form;
            ViewBag.TransportationTypes = transportationTypesRepo.FindAll().OrderBy(t => t.Name);
            ViewBag.Action = Url.ItineraryEditTransportation();
            return PartialView("TransportationDialog");
        }

        [HttpPost]
        public ActionResult EditTransportation(TransportationActivityForm form, User currentUser)
        {
            var activity = (TransportationActivity)activitiesRepo.Find(form.ActivityId.Value);
            var oldTrip = activity.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            if (!currentUser.OwnsTrips(oldTrip, newTrip)) return Redirect("/");

            activitiesRepo.Save(form, currentUser);
            var response = new { status = "OK" };
            return Json(response);
        }

        #endregion

        #region Places

        private ActionResult AddPlace(int? placeId, User currentUser)
        {
            Place place;

            if (placeId.HasValue)
            {
                var placesRepo = new PlacesRepo();
                place = placesRepo.Find(placeId.Value);
            }
            else
            {
                place = new Place();
            }

            PlaceActivityForm form = new PlaceActivityForm
            {
                Name = place.Name,
                GoogReference = place.GoogReference,
                GoogId = place.GoogId,
                TripId = currentUser.DefaultTrip.Id
            };
            ViewBag.Form = form;
            ViewBag.Place = place;
            ViewBag.Action = Url.ItineraryAddPlace();
            return PartialView("PlaceDialog");
        }

        [HttpPost]
        public ActionResult AddPlace(PlaceActivityForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            if (!currentUser.OwnsTrips(trip)) return Redirect("/");

            activitiesRepo.Save(form, currentUser);
            var response = new { status = "OK" };
            return Json(response);
        }

        private ActionResult EditPlace(PlaceActivity activity, ActivityForm.Tabs selectedTab)
        {
            var form = new PlaceActivityForm();
            form.SetBaseProps(activity);
            form.SelectedTab = selectedTab;

            if (activity.Place != null)
            {
                form.Name = activity.Place.Name;
                form.GoogReference = activity.Place.GoogReference;
                form.GoogId = activity.Place.GoogId;
            }

            ViewBag.Form = form;
            ViewBag.Place = activity.Place;
            ViewBag.Action = Url.ItineraryEditPlace();
            return PartialView("PlaceDialog");
        }

        [HttpPost]
        public ActionResult EditPlace(PlaceActivityForm form, User currentUser)
        {
            PlaceActivity activity = (PlaceActivity)activitiesRepo.Find(form.ActivityId.Value);
            var oldTrip = activity.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            if (!currentUser.OwnsTrips(oldTrip, newTrip)) return Redirect("/");

            activitiesRepo.Save(form, currentUser);
            var response = new { status = "OK" };
            return Json(response);
        }

        #endregion
    }
}