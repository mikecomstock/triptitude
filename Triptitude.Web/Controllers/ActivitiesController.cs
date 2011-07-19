﻿using System.Collections.Generic;
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

        #region Hotels

        public ActionResult AddHotel(int hotelId, User currentUser)
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
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(trip));
        }

        public ActionResult EditHotel(int activityId, User currentUser)
        {
            HotelActivity activity = (HotelActivity)activitiesRepo.Find(activityId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, activity.Trip);
            if (!userOwnsTrip) return Redirect("/");

            var form = new HotelActivityForm
            {
                ActivityId = activityId,
                BeginDay = activity.BeginDay,
                EndDay = activity.EndDay,
                TripId = activity.Trip.Id,
                HotelId = activity.Hotel.Id
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
            if (!userOwnsTrips) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(activity.Trip));
        }

        public ActionResult DeleteHotel(int activityId, User currentUser)
        {
            var activity = activitiesRepo.Find(activityId);
            var trip = activity.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Delete(activity);
            activitiesRepo.Save();
            return Redirect(Url.Details(trip));
        }

        #endregion

        #region Websites

        public ActionResult AddWebsite(User currentUser, int tripId)
        {
            WebsiteActivityForm form = new WebsiteActivityForm { TripId = tripId };
            ViewBag.Form = form;
            ViewBag.Action = Url.ItineraryAddWebsite();
            return PartialView("websitedialog");
        }

        [HttpPost]
        public ActionResult AddWebsite(WebsiteActivityForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(trip));
        }

        public ActionResult EditWebsite(int activityId, User currentUser)
        {
            var activity = (WebsiteActivity)activitiesRepo.Find(activityId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, activity.Trip);
            if (!userOwnsTrip) return Redirect("/");

            WebsiteActivityForm form = new WebsiteActivityForm
            {
                BeginDay = activity.BeginDay,
                EndDay = activity.EndDay,
                ActivityId = activityId,
                TripId = activity.Trip.Id,
                Url = activity.URL
            };
            ViewBag.Form = form;
            ViewBag.Action = Url.ItineraryEditWebsite();
            return PartialView("websitedialog");
        }

        [HttpPost]
        public ActionResult EditWebsite(WebsiteActivityForm form, User currentUser)
        {
            var activity = activitiesRepo.Find(form.ActivityId.Value);
            var oldTrip = activity.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(activity.Trip));
        }

        public ActionResult DeleteWebsite(int activityId, User currentUser)
        {
            var itineraryItem = activitiesRepo.Find(activityId);
            var trip = itineraryItem.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Delete(itineraryItem);
            activitiesRepo.Save();
            return Redirect(Url.Details(trip));
        }

        #endregion

        #region Destination Tags

        public ActionResult AddDestinationTag(User currentUser, int tripId)
        {
            TagActivityForm form = new TagActivityForm { TripId = tripId };
            ViewBag.Form = form;
            ViewBag.Action = Url.ItineraryAddDestinationTag();
            return PartialView("destinationtagdialog");
        }

        [HttpPost]
        public ActionResult AddDestinationTag(TagActivityForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(trip));
        }

        public ActionResult EditDestinationTag(int activityId, User currentUser)
        {
            var activity = (TagActivity)activitiesRepo.Find(activityId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, activity.Trip);
            if (!userOwnsTrip) return Redirect("/");

            TagActivityForm form = new TagActivityForm
                                          {
                                              BeginDay = activity.BeginDay,
                                              EndDay = activity.EndDay,
                                              ActivityId = activity.Id,
                                              TripId = activity.Trip.Id,
                                              CityId = activity.City.GeoNameID,
                                              CityName = activity.City.FullName,
                                              TagName = activity.Tag.Name
                                          };
            ViewBag.Form = form;
            ViewBag.Action = Url.ItineraryEditDestinationTag();
            return PartialView("destinationtagdialog");
        }

        [HttpPost]
        public ActionResult EditDestinationTag(TagActivityForm form, User currentUser)
        {
            var tagActivity = (TagActivity)activitiesRepo.Find(form.ActivityId.Value);
            var oldTrip = tagActivity.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(tagActivity.Trip));
        }

        public ActionResult DeleteDestinationTag(int activityId, User currentUser)
        {
            var itineraryItem = activitiesRepo.Find(activityId);
            var trip = itineraryItem.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Delete(itineraryItem);
            activitiesRepo.Save();
            return Redirect(Url.Details(trip));
        }

        #endregion

        #region Transportation

        public ActionResult AddTransportation(User currentUser, int tripId)
        {
            var fly = transportationTypesRepo.FindAll().First(tt => tt.Name == "Fly");
            TransportationActivityForm form = new TransportationActivityForm { TripId = tripId, TransportationTypeId = fly.Id };
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

            var transportation = activitiesRepo.Save(form);
            return Redirect(Url.Details(transportation.Trip));
        }

        public ActionResult EditTransportation(int activityId, User currentUser)
        {
            var transportation = (TransportationActivity)activitiesRepo.Find(activityId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, transportation.Trip);
            if (!userOwnsTrip) return Redirect("/");

            TransportationActivityForm form = new TransportationActivityForm
            {
                ActivityId = transportation.Id,
                TransportationTypeId = transportation.TransportationType.Id,
                TripId = transportation.Trip.Id,
                FromCityId = transportation.FromCity.GeoNameID,
                FromCityName = transportation.FromCity.FullName,
                ToCityId = transportation.ToCity.GeoNameID,
                ToCityName = transportation.ToCity.FullName,
                BeginDay = transportation.BeginDay,
                EndDay = transportation.EndDay
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
            if (!userOwnsTrips) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(activity.Trip));
        }

        public ActionResult DeleteTransportation(int activityId, User currentUser)
        {
            var transportation = activitiesRepo.Find(activityId);
            var trip = transportation.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Delete(transportation);
            activitiesRepo.Save();
            return Redirect(Url.Details(trip));
        }

        #endregion

        #region Places

        public ActionResult AddPlace(string placeId, User currentUser)
        {
            var placesService = new PlacesService();
            var place = placesService.Find(placeId);
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
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(trip));
        }

        public ActionResult EditPlace(int activityId, User currentUser)
        {
            PlaceActivity activity = (PlaceActivity)activitiesRepo.Find(activityId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, activity.Trip);
            if (!userOwnsTrip) return Redirect("/");

            var form = new PlaceActivityForm()
            {
                ActivityId = activityId,
                BeginDay = activity.BeginDay,
                EndDay = activity.EndDay,
                TripId = activity.Trip.Id,
                FactualId = activity.Place.FactualId
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
            if (!userOwnsTrips) return Redirect("/");

            activitiesRepo.Save(form);
            return Redirect(Url.Details(activity.Trip));
        }

        public ActionResult DeletePlace(int activityId, User currentUser)
        {
            var activity = activitiesRepo.Find(activityId);
            var trip = activity.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            activitiesRepo.Delete(activity);
            activitiesRepo.Save();
            return Redirect(Url.Details(trip));
        }
        #endregion
    }
}