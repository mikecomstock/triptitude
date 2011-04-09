using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class TransportationsController : Controller
    {
        private TransportationsRepo repo;
        private TransportationTypesRepo transportationTypesRepo;
        private TripsRepo tripsRepo;

        public TransportationsController()
        {
            repo = new TransportationsRepo();
            tripsRepo = new TripsRepo();
            transportationTypesRepo = new TransportationTypesRepo();
        }

        public ActionResult Create(User currentUser, int tripId)
        {
            TransportationForm form = new TransportationForm { TripId = tripId, TransportationTypeId = -1 };
            ViewBag.Form = form;
            ViewBag.TransportationTypes = transportationTypesRepo.FindAll().OrderBy(t => t.Name);
            ViewBag.Action = Url.CreateTransportation();
            return PartialView("Dialog");
        }

        [HttpPost]
        public ActionResult Create(TransportationForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return Redirect("/");

            var transportation = repo.Save(form);
            return Redirect(Url.Details(transportation.Trip));
        }

        public ActionResult Edit(int id, User currentUser)
        {
            var transportation = repo.Find(id);
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
            ViewBag.Action = Url.Edit(transportation);
            return PartialView("Dialog");
        }

        [HttpPost]
        public ActionResult Edit(TransportationForm form, User currentUser)
        {
            var transportation = repo.Find(form.Id.Value);
            var oldTrip = transportation.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsTrips = PermissionHelper.UserOwnsTrips(currentUser, oldTrip, newTrip);
            if (!userOwnsTrips) Redirect("/");

            repo.Save(form);
            return Redirect(Url.Details(transportation.Trip));
        }

        public ActionResult Delete(int id, User currentUser)
        {
            var transportation = repo.Find(id);
            var trip = transportation.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) Redirect("/");

            repo.Delete(transportation);
            repo.Save();
            return Redirect(Url.Details(trip));
        }
    }
}