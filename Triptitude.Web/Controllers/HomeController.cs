using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class HomeController : TriptitudeController
    {
        public ActionResult Index()
        {
            return View();
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

        public ActionResult Attribution()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 410;
            return View();
        }
    }
}
