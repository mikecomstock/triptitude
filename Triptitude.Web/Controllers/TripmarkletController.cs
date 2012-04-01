using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class TripmarkletController : TriptitudeController
    {
        public ActionResult Tripmarklet(string url, string title)
        {
            if (CurrentUser.DefaultTrip == null)
                return Redirect("/tripmarklet/createtrip");


            ViewBag.URL = url;
            ViewBag.ParsedTitle = title;

            var currentUserData = new
                                      {
                                          Email = CurrentUser.Email,
                                          DefaultTripID = CurrentUser.DefaultTrip.Id,
                                          Trips = from t in CurrentUser.Trips//.Where(t => t.Id == 194)
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

        [HttpGet]
        public ActionResult CreateTrip(string returnURL)
        {
            ViewBag.Form = new NewCreateTripForm();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTrip(NewCreateTripForm form)
        {
            if (ModelState.IsValid)
            {
                Trip trip = new TripsRepo().Save(form, CurrentUser);
                new UsersRepo().SetDefaultTrip(CurrentUser, trip);
                new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Created, HistoryTable.Trips, trip.Id);
                return Redirect("/tripmarklet/tripmarklet");
            }
            else
            {
                ViewBag.Form = form;
                return View();
            }
        }
    }
}