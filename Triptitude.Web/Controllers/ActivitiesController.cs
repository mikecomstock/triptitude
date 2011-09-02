using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly TripsRepo tripsRepo;
        private readonly ActivitiesRepo activitiesRepo;
        private readonly HotelsRepo hotelsRepo;
        private readonly TransportationTypesRepo transportationTypesRepo;

        public ActivitiesController()
        {
            tripsRepo = new TripsRepo();
            activitiesRepo = new ActivitiesRepo();
            hotelsRepo = new HotelsRepo();
            transportationTypesRepo = new TransportationTypesRepo();
        }

        public ActionResult Delete(int id, User currentUser)
        {
            var activity = activitiesRepo.Find(id);
            var trip = activity.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

            activitiesRepo.Delete(activity);
            activitiesRepo.Save();
            return Redirect(Url.Details(trip));
        }

        public ActionResult Edit(int id, User currentUser, ActivityForm.Tabs selectedTab = ActivityForm.Tabs.Details)
        {
            var activity = activitiesRepo.Find(id);
            var trip = activity.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

            if (activity is TransportationActivity) return EditTransportation(activity as TransportationActivity, selectedTab);
            if (activity is HotelActivity) return EditHotel(activity as HotelActivity, selectedTab);
            if (activity is PlaceActivity) return EditPlace(activity as PlaceActivity, selectedTab);

            throw new Exception("Activity type not supported");
        }

        public ActionResult Create(string type, User currentUser, string placeId, int? hotelId)
        {
            switch (type)
            {
                case "transportation": return AddTransportation(currentUser);
                case "place": return AddPlace(placeId, currentUser);
                case "hotel": return AddHotel(hotelId.Value, currentUser);
            }

            throw new Exception("Activity type not supported");
        }

        #region Hotels

        private ActionResult AddHotel(int hotelId, User currentUser)
        {
            var hotel = hotelsRepo.Find(hotelId);
            HotelActivityForm form = new HotelActivityForm
            {
                TripId = currentUser.DefaultTrip.Id,
                HotelId = hotelId
            };
            ViewBag.Form = form;
            ViewBag.Hotel = hotel;
            ViewBag.Action = Url.ItineraryAddHotel();
            return PartialView("HotelDialog");
        }

        [HttpPost]
        public ActionResult AddHotel(HotelActivityForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

            activitiesRepo.Save(form, currentUser);
            return Redirect(Url.Details(trip));
        }

        private ActionResult EditHotel(HotelActivity activity, ActivityForm.Tabs selectedTab)
        {
            var form = new HotelActivityForm
            {
                ActivityId = activity.Id,
                BeginDay = activity.BeginDay,
                EndDay = activity.EndDay,
                TripId = activity.Trip.Id,
                HotelId = activity.Hotel.Id,
                Notes = activity.Notes,
                SelectedTab = selectedTab
            };
            ViewBag.Form = form;
            ViewBag.Hotel = activity.Hotel;
            ViewBag.Action = Url.ItineraryEditHotel();
            return PartialView("HotelDialog");
        }

        [HttpPost]
        public ActionResult EditHotel(HotelActivityForm form, User currentUser)
        {
            HotelActivity activity = (HotelActivity)activitiesRepo.Find(form.ActivityId.Value);
            var oldTrip = activity.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) return PartialView("WorkingOnIt");// Redirect("/");

            activitiesRepo.Save(form, currentUser);
            return Redirect(Url.Details(activity.Trip));
        }

        #endregion

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
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            var transportation = activitiesRepo.Save(form, currentUser);
            return Redirect(Url.Details(transportation.Trip));
        }

        private ActionResult EditTransportation(TransportationActivity activity, ActivityForm.Tabs selectedTab)
        {
            TransportationActivityForm form = new TransportationActivityForm
            {
                ActivityId = activity.Id,
                TransportationTypeId = activity.TransportationType.Id,
                TripId = activity.Trip.Id,
                FromCityId = activity.FromCity.GeoNameID,
                FromCityName = activity.FromCity.FullName,
                ToCityId = activity.ToCity.GeoNameID,
                ToCityName = activity.ToCity.FullName,
                BeginDay = activity.BeginDay,
                EndDay = activity.EndDay,
                Notes = activity.Notes,
                SelectedTab = selectedTab
            };
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
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) return PartialView("WorkingOnIt");// Redirect("/");

            activitiesRepo.Save(form, currentUser);
            return Redirect(Url.Details(activity.Trip));
        }

        #endregion

        #region Places

        /// <summary>
        /// Note that placeId refers to a FactualId. It is null when adding a custom place.
        /// </summary>
        private ActionResult AddPlace(string placeId, User currentUser)
        {
            Place place;
            if (placeId == "")
            {
                place = new Place();
                place.Name = "test";
            }
            else
            {
                var placesService = new PlacesService();
                place = placesService.Find(placeId);
            }

            PlaceActivityForm form = new PlaceActivityForm
                                         {
                                             FactualId = placeId,
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
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

            activitiesRepo.Save(form, currentUser);
            return Redirect(Url.Details(trip));
        }

        private ActionResult EditPlace(PlaceActivity activity, ActivityForm.Tabs selectedTab)
        {
            var form = new PlaceActivityForm()
            {
                ActivityId = activity.Id,
                BeginDay = activity.BeginDay,
                EndDay = activity.EndDay,
                TripId = activity.Trip.Id,
                TagName = activity.Tags.Any() ? activity.Tags.First().Name : string.Empty,
                FactualId = activity.Place.FactualId,
                PlaceId = activity.Place.Id,
                Name = activity.Place.Name,
                Address = activity.Place.Address,
                Telephone = activity.Place.Telephone,
                Website = activity.Place.Website,
                Latitude = activity.Place.Latitude.HasValue ? activity.Place.Latitude.Value.ToString() : string.Empty,
                Longitude = activity.Place.Longitude.HasValue ? activity.Place.Longitude.Value.ToString() : string.Empty,
                Notes = activity.Notes,
                SelectedTab = selectedTab
            };
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
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) return PartialView("WorkingOnIt");// Redirect("/");

            activitiesRepo.Save(form, currentUser);
            return Redirect(Url.Details(activity.Trip));
        }

        #endregion
    }
}