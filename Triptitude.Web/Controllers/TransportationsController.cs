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

        public TransportationsController()
        {
            repo = new TransportationsRepo();
        }

        public ActionResult Edit(int id, User currentUser)
        {
            var transportation = repo.Find(id);
            bool userOwnsTrip = transportation.Trip.Users.Contains(currentUser);
            if (!userOwnsTrip) return Redirect("/");

            ViewBag.Transportation = transportation;
            TransportationForm form = new TransportationForm
                                          {
                                              Id = transportation.Id,
                                              TripId = transportation.Trip.Id,
                                              FromCityId = transportation.FromCity.Id,
                                              FromCityName = transportation.FromCity.FullName,
                                              ToCityId = transportation.ToCity.Id,
                                              ToCityName = transportation.ToCity.FullName,
                                              BeginDay = transportation.BeginDay,
                                              EndDay = transportation.EndDay
                                          };
            ViewBag.Form = form;
            return PartialView();
        }

        [HttpPost]
        public ActionResult Edit(TransportationForm form, User currentUser)
        {
            var tripsRepo = new TripsRepo();
            var citiesRepo = new CitiesRepo();

            var transportation = repo.Find(form.Id);
            var oldTrip = transportation.Trip;
            var newTrip = tripsRepo.Find(form.TripId);
            bool userOwnsOldTrip = oldTrip.Users.Contains(currentUser);
            bool userOwnsNewTrip = newTrip.Users.Contains(currentUser);

            if (!userOwnsOldTrip || !userOwnsNewTrip) Redirect("/");

            transportation.Trip = newTrip;
            transportation.ToCity = citiesRepo.Find(form.ToCityId);
            transportation.FromCity = citiesRepo.Find(form.FromCityId);
            transportation.BeginDay = form.BeginDay;
            transportation.EndDay = form.EndDay;
            repo.Save();

            return Redirect(Url.PublicDetails(newTrip));
        }
    }
}