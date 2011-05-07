using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class DestinationsController : Controller
    {
        DestinationsRepo destinationsRepo;

        public DestinationsController()
        {
            destinationsRepo = new DestinationsRepo();
        }

        public ActionResult Redirect(int id)
        {
            IDestination destination = destinationsRepo.Find(id);
            return RedirectPermanent(Url.Details(destination));
        }

        public ActionResult _SidePanel(int id)
        {
            IDestination destination = destinationsRepo.Find(id);
            ViewBag.Destination = destination;
            return PartialView();
        }

        public ActionResult Details(int id)
        {
            IDestination destination = destinationsRepo.Find(id);
            ViewBag.Destination = destination;
            ViewBag.Trips = new TripsRepo().FindAll().Take(5);
            return View();
        }

        public ActionResult Hotels(int id)
        {
            int radiusInMiles = 50;
            int radiusInMeters = (int)(radiusInMiles * 1609.344);

            IDestination destination = destinationsRepo.Find(id);
            ViewBag.Destination = destination;
            ViewBag.Hotels = new HotelsRepo().FindNear((City)destination, radiusInMeters).Take(40);
            return View();
        }

        public ActionResult Activities(int id)
        {
            IDestination destination = destinationsRepo.Find(id);
            ViewBag.Destination = destination;
            var tags = destination.Tags;
            ViewBag.Tags = tags;
            return View();
        }

        // JSON only
        public ActionResult Search(string term)
        {
            var luceneService = new LuceneService();
            var searchResults = luceneService.SearchDestinations(term);

            var results = from d in searchResults
                          select new
                                     {
                                         label = d.FullName,
                                         id = d.GeoNameId
                                     };
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(int destinationId)
        {
            IDestination destination = destinationsRepo.Find(destinationId);
            return Redirect(Url.Details(destination));
        }
    }
}
