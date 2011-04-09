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

        public ItineraryItemsController()
        {
            tripsRepo = new TripsRepo();
            itineraryItemsRepo = new ItineraryItemsRepo();
            hotelsRepo = new HotelsRepo();
        }

        public ActionResult Edit(int id)
        {
            var itineraryItem = itineraryItemsRepo.Find(id);
            var itineraryItemSettings = itineraryItemsRepo.GetSettings(itineraryItem);

            ViewBag.ItineraryItem = itineraryItem;
            ViewBag.Settings = itineraryItemSettings;

            return PartialView();
        }

        [HttpPost]
        public ActionResult Edit(int id, ItineraryItemSettings settings)
        {
            ItineraryItem itineraryItem = itineraryItemsRepo.Find(id);
            itineraryItemsRepo.Save(itineraryItem, settings);
            return Redirect(Url.Details(itineraryItem.Trip));
        }

        public ActionResult Delete(int id)
        {
            ItineraryItem itineraryItem = itineraryItemsRepo.Find(id);
            var trip = itineraryItem.Trip;
            itineraryItemsRepo.Delete(itineraryItem);
            itineraryItemsRepo.Save();
            return Redirect(Url.Details(trip));
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
    }
}