using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class HomeController : TriptitudeController
    {
        public ActionResult Index()
        {
            TripsRepo tripsRepo = new TripsRepo();
            var trips = tripsRepo.FindAll().Where(t => t.ShowInSearch).OrderByDescending(t => t.Created_On).Take(10).ToList();
            ViewBag.HideTripBar = true;
            ViewBag.Trips = trips;

            return View();
        }

        public ActionResult Header(bool? hideTripBar)
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.HideTripBar = hideTripBar;
            return PartialView();
        }


        public ActionResult Sitemap()
        {
            var tripsRepo = new TripsRepo();
            var trips = tripsRepo.FindAll().Where(t => t.ShowInSearch);
            ViewBag.Trips = trips;

            var itemTagRepo = new ItemTagRepo();
            var tags = itemTagRepo.FindAll().Where(it => it.ShowInSearch).Select(it => it.Tag).Distinct().Where(t => t != null);
            ViewBag.Tags = tags;

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
