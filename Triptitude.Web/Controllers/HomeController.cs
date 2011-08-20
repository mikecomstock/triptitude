using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
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
            ViewBag.HideTripBar = true;
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

            List<City> allCities = new List<City>();
            var places = trips.Where(t => t.ShowInSiteMap).SelectMany(t => t.Activities).OfType<PlaceActivity>().Select(pa => pa.Place).Where(p => p.Latitude != null && p.Longitude != null).Distinct();
            CitiesRepo citiesRepo = new CitiesRepo();
            foreach (var place in places)
            {
                IEnumerable<City> nearbyCities = citiesRepo.Search(new CitySearchForm { Latitude = place.Latitude.Value, Longitude = place.Longitude.Value, RadiusInMiles = 10 }).ToList();
                allCities.AddRange(nearbyCities);
            }
            allCities = allCities.Distinct().ToList();
            ViewBag.Destinations = allCities;

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
