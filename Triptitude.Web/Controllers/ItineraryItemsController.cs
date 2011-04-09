using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class ItineraryItemsController : Controller
    {
        private TripsRepo tripsRepo;
        private ItineraryItemsRepo itineraryItemsRepo;
        private HotelsRepo hotelsRepo;
        private TransportationsRepo transportationsRepo;
        private TransportationTypesRepo transportationTypesRepo;

        public ItineraryItemsController()
        {
            tripsRepo = new TripsRepo();
            itineraryItemsRepo = new ItineraryItemsRepo();
            hotelsRepo = new HotelsRepo();
            transportationTypesRepo = new TransportationTypesRepo();
            transportationsRepo = new TransportationsRepo();
        }

        #region Hotels

        public ActionResult AddHotel(int hotelId, User currentUser)
        {
            var hotel = hotelsRepo.Find(hotelId);
            HotelForm form = new HotelForm
            {
                TripId = currentUser.DefaultTrip.Id,
                HotelId = hotelId
            };
            ViewBag.Form = form;
            ViewBag.Hotel = hotel;
            ViewBag.Action = Url.ItineraryAddHotel(hotel);
            return PartialView("HotelDialog");
        }

        [HttpPost]
        public ActionResult AddHotel(HotelForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            itineraryItemsRepo.Save(form);
            return Redirect(Url.Details(trip));
        }

        public ActionResult EditHotel(int itineraryItemId, User currentUser)
        {
            var itineraryItem = itineraryItemsRepo.Find(itineraryItemId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, itineraryItem.Trip);
            if (!userOwnsTrip) return Redirect("/");

            var form = new HotelForm()
                        {
                            BeginDay = itineraryItem.BeginDay,
                            EndDay = itineraryItem.EndDay,
                            ItineraryItemId = itineraryItemId,
                            TripId = itineraryItem.Trip.Id,
                            HotelId = itineraryItem.Hotel.Id
                        };
            ViewBag.Form = form;
            ViewBag.Hotel = itineraryItem.Hotel;
            ViewBag.Action = Url.ItineraryEditHotel(itineraryItem);
            return PartialView("HotelDialog");
        }

        [HttpPost]
        public ActionResult EditHotel(HotelForm form, User currentUser)
        {
            var itineraryItem = itineraryItemsRepo.Find(form.ItineraryItemId.Value);
            var oldTrip = itineraryItem.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) Redirect("/");

            itineraryItemsRepo.Save(form);
            return Redirect(Url.Details(itineraryItem.Trip));
        }

        public ActionResult DeleteHotel(int itineraryItemId, User currentUser)
        {
            var itineraryItem = itineraryItemsRepo.Find(itineraryItemId);
            var trip = itineraryItem.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) Redirect("/");

            itineraryItemsRepo.Delete(itineraryItem);
            itineraryItemsRepo.Save();
            return Redirect(Url.Details(trip));
        }

        #endregion

        #region Websites

        public ActionResult AddWebsite(User currentUser, int tripId)
        {
            WebsiteForm form = new WebsiteForm { TripId = tripId };
            ViewBag.Form = form;
            ViewBag.Action = Url.ItineraryAddWebsite();
            return PartialView("websitedialog");
        }

        [HttpPost]
        public ActionResult AddWebsite(WebsiteForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            itineraryItemsRepo.Save(form);
            return Redirect(Url.Details(trip));
        }

        public ActionResult EditWebsite(int itineraryItemId, User currentUser)
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
            ViewBag.Action = Url.ItineraryEditWebsite();
            return PartialView("websitedialog");
        }

        [HttpPost]
        public ActionResult EditWebsite(WebsiteForm form, User currentUser)
        {
            var itineraryItem = itineraryItemsRepo.Find(form.ItineraryItemId.Value);
            var oldTrip = itineraryItem.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) Redirect("/");

            itineraryItemsRepo.Save(form);
            return Redirect(Url.Details(itineraryItem.Trip));
        }

        public ActionResult DeleteWebsite(int itineraryItemId, User currentUser)
        {
            var itineraryItem = itineraryItemsRepo.Find(itineraryItemId);
            var trip = itineraryItem.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) Redirect("/");

            itineraryItemsRepo.Delete(itineraryItem);
            itineraryItemsRepo.Save();
            return Redirect(Url.Details(trip));
        }

        #endregion

        #region Transportation

        public ActionResult AddTransportation(User currentUser, int tripId)
        {
            TransportationForm form = new TransportationForm { TripId = tripId, TransportationTypeId = -1 };
            ViewBag.Form = form;
            ViewBag.TransportationTypes = transportationTypesRepo.FindAll().OrderBy(t => t.Name);
            ViewBag.Action = Url.ItineraryAddTransportation();
            return PartialView("TransportationDialog");
        }

        [HttpPost]
        public ActionResult AddTransportation(TransportationForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            var transportation = transportationsRepo.Save(form);
            return Redirect(Url.Details(transportation.Trip));
        }

        public ActionResult EditTransportation(int id, User currentUser)
        {
            var transportation = transportationsRepo.Find(id);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, transportation.Trip);
            if (!userOwnsTrip) return Redirect("/");

            TransportationForm form = new TransportationForm
            {
                Id = transportation.Id,
                TransportationTypeId = transportation.TransportationType.Id,
                TripId = transportation.Trip.Id,
                FromCityId = transportation.FromCity.Id,
                FromCityName = transportation.FromCity.FullName,
                ToCityId = transportation.ToCity.Id,
                ToCityName = transportation.ToCity.FullName,
                BeginDay = transportation.BeginDay,
                EndDay = transportation.EndDay
            };
            ViewBag.Form = form;
            ViewBag.TransportationTypes = transportationTypesRepo.FindAll().OrderBy(t => t.Name);
            ViewBag.Action = Url.ItineraryEditTransportation(transportation);
            return PartialView("TransportationDialog");
        }

        [HttpPost]
        public ActionResult EditTransportation(TransportationForm form, User currentUser)
        {
            var transportation = transportationsRepo.Find(form.Id.Value);
            var oldTrip = transportation.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) Redirect("/");

            transportationsRepo.Save(form);
            return Redirect(Url.Details(transportation.Trip));
        }

        public ActionResult DeleteTransportation(int id, User currentUser)
        {
            var transportation = transportationsRepo.Find(id);
            var trip = transportation.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) Redirect("/");

            transportationsRepo.Delete(transportation);
            transportationsRepo.Save();
            return Redirect(Url.Details(trip));
        }

        #endregion
    }
}