using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
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
            if (destination is City)
                ViewBag.Trips = (destination as City).Activities.Select(a => a.Trip).Where(t => t.ShowInSite).Distinct();
            return View();
        }

        public ActionResult Hotels(int id)
        {
            City city = (City)destinationsRepo.Find(id);
            ViewBag.Destination = city;
            var hotelSearchForm = new HotelSearchForm { Latitude = city.Latitude, Longitude = city.Longitude, RadiusInMiles = 10 };
            ViewBag.HotelSearchForm = hotelSearchForm;

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
        public ActionResult Search(string term, User currentUser)
        {
            IEnumerable<City> citiesInTrip = new List<City>();
            if (currentUser.DefaultTrip != null)
            {
                var firstTerm = term.Split(' ')[0];
                citiesInTrip = currentUser.DefaultTrip.Cities.Where(c => c.ShortName.StartsWith(firstTerm, StringComparison.InvariantCultureIgnoreCase));
            }

            var tripCities = from t in citiesInTrip
                             select new
                                        {
                                            label = t.FullName,
                                            id = t.GeoNameID
                                        };

            var luceneService = new LuceneService();
            var searchResults = luceneService.SearchDestinations(term);

            var results = from d in searchResults.Where(sr => !citiesInTrip.Select(c => c.GeoNameID).Contains(sr.GeoNameId))
                          select new
                                     {
                                         label = d.FullName,
                                         id = d.GeoNameId
                                     };

            return Json(tripCities.Union(results).Take(10), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(int destinationId)
        {
            IDestination destination = destinationsRepo.Find(destinationId);
            return Redirect(Url.Details(destination));
        }
    }
}
