using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class DestinationsController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Cities = new CitiesRepo().FindAll().Where(c => c.Region.ASCIIName == "minnesota").Take(10);
            ViewBag.Regions = new RegionsRepo().FindAll().Take(10);
            ViewBag.Countries = new CountriesRepo().FindAll().OrderBy(c => c.Name);
            return View();
        }

        public ActionResult Details(int id)
        {
            DestinationsRepo destinationsRepo = new DestinationsRepo();
            Destination destination = destinationsRepo.Find(id);
            ViewBag.Destination = destination;
            ViewBag.Trips = new TripsRepo().FindAll().Take(5);
            return View();
        }
    }
}
