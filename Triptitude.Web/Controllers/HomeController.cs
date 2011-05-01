using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Header(User currentUser, bool? hideTripBar)
        {
            ViewBag.User = currentUser;
            ViewBag.HideTripBar = hideTripBar;
            return PartialView();
        }

        public ActionResult Search(string s)
        {
            ViewBag.SearchString = s;
            return PartialView("_SearchResults");
        }

        public ActionResult Sitemap()
        {
            TripsRepo tripsRepo = new TripsRepo();
            IQueryable<Trip> trips = tripsRepo.FindAll();
            ViewBag.Trips = trips;
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 410;
            return View();
        }
    }
}
