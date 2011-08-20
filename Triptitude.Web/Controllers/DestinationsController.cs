﻿using System;
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
            {
                City city = destination as City;
                TripsRepo tripsRepo = new TripsRepo();
                var trips = tripsRepo.Search(new TripSearchForm { Latitude = city.Latitude, Longitude = city.Longitude, RadiusInMiles = 50 });
                trips = trips.Where(t => t.ShowInSite);
                ViewBag.Trips = trips;
            }
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
            City city = (City)destinationsRepo.Find(id);
            ViewBag.Destination = city;
            ViewBag.Tags = city.TagsToShow;
            return View();
        }

        public ActionResult Activity(int id, int tagId)
        {
            City city = (City)destinationsRepo.Find(id);
            ViewBag.Destination = city;

            var tagsRepo = new TagsRepo();
            ViewBag.Tag = tagsRepo.Find(tagId);
            return View();
        }

        public ActionResult Places(int id)
        {
            City city = (City)destinationsRepo.Find(id);
            ViewBag.Destination = city;
            var placeSearchForm = new PlaceSearchForm { Latitude = city.Latitude, Longitude = city.Longitude, RadiusInMiles = 10 };
            ViewBag.PlaceSearchForm = placeSearchForm;
            return View();
        }

        // JSON only
        public ActionResult Search(string term, User currentUser)
        {
            IEnumerable<City> citiesInTrip = new List<City>();
            //TODO: fix this
            //if (currentUser.DefaultTrip != null)
            //{
            //    var firstTerm = term.Split(' ')[0];
            //    citiesInTrip = currentUser.DefaultTrip.Cities.Where(c => c.ShortName.StartsWith(firstTerm, StringComparison.InvariantCultureIgnoreCase));
            //}

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
