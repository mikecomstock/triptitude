using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    [Authorize(Users = "1|mikecomstock@gmail.com")]
    public class AdminController : Controller
    {
        private TripsRepo tripsRepo;

        public AdminController()
        {
            tripsRepo = new TripsRepo();
        }

        public ActionResult Index()
        {
            var trips = tripsRepo.FindAll();
            ViewBag.Trips = trips;
            return View();
        }

        [HttpGet]
        public ActionResult Trips(int id)
        {
            var trip = tripsRepo.Find(id);
            ViewBag.Trip = trip;
            return View();
        }

        [HttpPost]
        public ActionResult Trips(int id, FormCollection form)
        {
            Trip trip = tripsRepo.Find(id);
            trip.Moderated = true;
            trip.ShowInSiteMap = form["trip.ShowInSiteMap"].Split(',')[0] == "true";
            trip.ShowInSite = form["trip.ShowInSite"].Split(',')[0] == "true";
            tripsRepo.Save();

            return Redirect("/admin");
        }
    }
}