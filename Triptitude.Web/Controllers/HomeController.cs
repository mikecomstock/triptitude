using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Razor.Parser;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
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

        public ActionResult Tripmarklet(string url, string title)
        {
            ViewBag.URL = url;
            ViewBag.ParsedTitle = title;

            var currentUserData = new
                                  {
                                      Email = CurrentUser.Email,
                                      DefaultTripID = CurrentUser.DefaultTrip.Id,
                                      Trips = from t in CurrentUser.Trips.Where(t => t.Id == 194)
                                              select new
                                                         {
                                                             ID = t.Id,
                                                             Name = t.Name,
                                                             Activities = from a in t.NonDeletedActivities
                                                                          select new
                                                                                     {
                                                                                         ID = a.Id,
                                                                                         Title = a.Title,
                                                                                         a.IsTransportation,
                                                                                         BeginAt = a.BeginAt.HasValue ? a.BeginAt.Value.ToString("MM/dd/yy") : string.Empty,
                                                                                         EndAt = a.EndAt.HasValue ? a.EndAt.Value.ToString("MM/dd/yy") : string.Empty,
                                                                                         TransportationTypeName = a.TransportationType == null ? string.Empty : a.TransportationType.Name,
                                                                                         SourceURL = a.SourceURL,
                                                                                         Places = from p in a.ActivityPlaces
                                                                                                  select new
                                                                                                             {
                                                                                                                 p.SortIndex,
                                                                                                                 p.Place.Id,
                                                                                                                 p.Place.Name
                                                                                                             }
                                                                                     }
                                                         }
                                  };

            ViewBag.CurrentUserData = currentUserData;
            //ViewBag.CurrentUserAsString = JsonConvert.SerializeObject(currentUser);

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
