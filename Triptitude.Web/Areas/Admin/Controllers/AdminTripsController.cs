using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Areas.Admin.Controllers
{
    [Authorize(Users = "1|mikecomstock@gmail.com")]
    public class AdminTripsController : Controller
    {
        private TripsRepo tripsRepo;
        public AdminTripsController()
        {
            tripsRepo = new TripsRepo();
        }

        public ActionResult Index()
        {
            var trips = tripsRepo.FindAll().OrderByDescending(t => t.Id);
            ViewBag.Trips = trips;
            return View();
        }

        public ActionResult Details(int id)
        {
            Trip trip = tripsRepo.Find(id);
            ViewBag.Trip = trip;
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection form)
        {
            Trip trip = tripsRepo.Find(id);
            trip.ModeratedOnUTC = DateTime.UtcNow;
            trip.ShowInSearch = form["trip.ShowInSearch"].Split(',')[0] == "true";
            tripsRepo.Save();

            return Redirect(Url.Admin("trips"));
        }
    }
}