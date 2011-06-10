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
            TripsRepo tripsRepo = new TripsRepo();
            var trips = tripsRepo.FindAll().Where(t => t.ShowInSite).OrderByDescending(t => t.Created_On).Take(10).ToList();
            ViewBag.Trips = trips;

            return View();
        }

        public ActionResult Header(User currentUser, bool? hideTripBar)
        {
            ViewBag.CurrentUser = currentUser;
            ViewBag.HideTripBar = hideTripBar;
            return PartialView();
        }


        public ActionResult Sitemap()
        {
            TripsRepo tripsRepo = new TripsRepo();
            IQueryable<Trip> trips = tripsRepo.FindAll().Where(t => t.ShowInSiteMap);
            ViewBag.Trips = trips;

            Response.ContentType = "text/xml";
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 410;
            return View();
        }
    }
}
