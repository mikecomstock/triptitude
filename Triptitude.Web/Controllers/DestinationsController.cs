using System.Collections.Generic;
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
        public ActionResult Details(int id)
        {
            DestinationsRepo destinationsRepo = new DestinationsRepo();
            Destination destination = destinationsRepo.Find(id);
            ViewBag.Destination = destination;
            ViewBag.Trips = new TripsRepo().FindAll().Take(5);
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
            Destination destination = new DestinationsRepo().Find(destinationId);
            return Redirect(Url.Details(destination));
        }
    }
}
