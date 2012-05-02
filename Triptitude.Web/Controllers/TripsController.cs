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
    public class TripsController : TriptitudeController
    {
        private TripsRepo repo;
        public TripsController()
        {
            repo = new TripsRepo();
        }

        // partial only
        public ActionResult SidePanel(Trip trip)
        {
            ViewBag.Trip = trip;
            ViewBag.UserOwnsTrip = CurrentUser.OwnsTrips(trip);
            return PartialView("_SidePanel");
        }

        // partial only
        public ActionResult _Row(Trip trip)
        {
            ViewBag.Trip = trip;
            return PartialView();
        }

        // partial
        public ActionResult _Rows(IEnumerable<Trip> trips)
        {
            ViewBag.Trips = trips;
            return PartialView();
        }

        public ActionResult Index()
        {
            var tripSearchForm = new TripSearchForm(take: 100);
            ViewBag.TripSearchForm = tripSearchForm;
            return View();
        }

        public ActionResult SearchResults(TripSearchForm form)
        {
            ViewBag.Trips = form.Results;
            return PartialView();
        }

        //public ActionResult Print(int id)
        //{
        //    Trip trip = new TripsRepo().Find(id);
        //    ViewBag.Trip = trip;
        //    return View();
        //}

        //public ActionResult Map(int id)
        //{
        //    Trip trip = new TripsRepo().Find(id);
        //    return RedirectPermanent(Url.Details(trip) + "#map");
        //    //ViewBag.Trip = trip;
        //    //return View();
        //}

        public ActionResult Details(int id)
        {
            var trip = repo.Find(id);
            if (trip == null) return HttpNotFound();
            if (trip.UserTrips.All(ut => ut.Deleted))
            {
                Response.StatusCode = 410; // Gone
                return Content("Sorry, this trip has been deleted.");
            }

            if (trip.UserTrips.All(ut => ut.Visibility == (byte)UserTrip.UserTripVisibility.Private) && !CurrentUser.OwnsTrips(trip) && !CurrentUser.IsAdmin)
            {
                //TODO: make nice 'permission denied' page
                Response.StatusCode = 403;
                return Content("Sorry, this trip is private. If this is your trip, please log in and try again.");
            }

            if (Request.IsAjaxRequest())
                return Json(trip.Json(CurrentUser), JsonRequestBehavior.AllowGet);

            ViewBag.Trip = trip;
            return View();
        }

        //public JsonResult Find(int activity_id)
        //{
        //    var activity = new ActivitiesRepo().Find(activity_id);
        //    var trip = activity.Trip;
        //    var json = trip.Json(CurrentUser);

        //    return Json(json, JsonRequestBehavior.AllowGet);
        //}

        // partial only
        public ActionResult DayDetails(Trip trip, int? dayNumber)
        {
            // For unscheduled activities
            dayNumber = dayNumber == -1 ? null : dayNumber;

            ViewBag.DayNumber = dayNumber;
            ViewBag.Trip = trip;
            ViewBag.Editing = CurrentUser.DefaultTrip == trip;
            ViewBag.CurrentUser = CurrentUser;
            return PartialView("_DayDetails");
        }

        public ActionResult PackingList(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            return RedirectPermanent(Url.Details(trip) + "#packinglist");
        }

        public ActionResult PackingListPartial(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            bool userOwnsTrip = CurrentUser.OwnsTrips(trip);
            ViewBag.UserOwnsTrip = userOwnsTrip;
            ViewBag.CurrentUser = CurrentUser;

            var packingListItems = trip.PackingListItems.Where(pli => userOwnsTrip || pli.Visibility == Visibility.Public).OrderBy(pli => pli.ItemTag.Item.Name);
            ViewBag.PackingListItems = packingListItems;
            var tags = packingListItems.Select(pli => pli.ItemTag).Select(it => it.Tag).Where(t => t != null).Distinct();
            ViewBag.Tags = tags;

            var itemIdsUsed = packingListItems.Select(pli => pli.ItemTag.Item.Id).Distinct();
            ViewBag.Suggestions = new ItemTagRepo().MostPopular().Where(it => !itemIdsUsed.Contains(it.Item.Id)).Take(9);
            return PartialView("PackingList");
        }

        public ActionResult Settings(int id)
        {
            var userTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == id);
            if (userTrip == null) return Redirect("/");
            var trip = ViewBag.Trip = userTrip.Trip;

            ViewBag.Form = new TripSettingsForm
            {
                Name = trip.Name,
                Visibility = (UserTrip.UserTripVisibility)Enum.Parse(typeof(UserTrip.UserTripVisibility), userTrip.Visibility.ToString())
            };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(int id, TripSettingsForm form)
        {
            var userTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == id);
            if (userTrip == null) return Redirect("/");
            var trip = userTrip.Trip;

            //Only the creator can change the name.
            if (CurrentUser.CreatedTrips(trip))
            {
                if (ModelState.IsValid) trip.Name = form.Name;
                else
                {
                    ViewBag.Trip = trip;
                    ViewBag.Form = form;
                    return View();
                }
            }

            userTrip.Visibility = (byte)form.Visibility;
            repo.Save();
            new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Modified, HistoryTable.Trips, trip.Id);
            TempData["saved"] = true;
            return Redirect(Url.Settings(trip));
        }

        public ActionResult Create(int? to)
        {
            var form = new NewCreateTripForm { Visibility = UserTrip.UserTripVisibility.Public };

            if (to.HasValue)
            {
                var placesRepo = new PlacesRepo();
                Place place = placesRepo.Find(to.Value);
                form.Name = "Trip to " + place.Name;
            }

            ViewBag.Form = form;
            return View();
        }

        [HttpPost]
        public ActionResult Create(NewCreateTripForm form)
        {
            if (ModelState.IsValid)
            {
                Trip trip = new Trip
                {
                    Name = form.Name,
                    Created_On = DateTime.UtcNow
                };

                repo.Add(trip);

                UserTrip userTrip = new UserTrip
                {
                    Trip = trip,
                    IsCreator = true,
                    Status = (byte)UserTripStatus.Attending,
                    StatusUpdatedOnUTC = DateTime.UtcNow,
                    User = CurrentUser,
                    Visibility = (byte)form.Visibility
                };
                trip.UserTrips.Add(userTrip);

                repo.Save();

                EmailService.SendTripCreated(trip);

                new UsersRepo().SetDefaultTrip(CurrentUser, trip);
                new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Created, HistoryTable.Trips, trip.Id);
                return Redirect(Url.Who(trip));
            }
            else
            {
                ViewBag.Form = form;
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            var userTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == id);
            if (userTrip == null) return Redirect("/");
            var trip = userTrip.Trip;

            userTrip.Deleted = true;

            if (CurrentUser.DefaultTrip.Id == trip.Id)
                CurrentUser.DefaultTrip = CurrentUser.Trips(CurrentUser).OrderByDescending(t => t.Id).FirstOrDefault();

            repo.Save();

            new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Deleted, HistoryTable.Trips, trip.Id);
            return Json(trip.Json(CurrentUser));
        }

        public ActionResult History(int id)
        {
            Trip trip = repo.Find(id);
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            ViewBag.Trip = repo.Find(id);
            return View();
        }

        public ActionResult Who(int id)
        {
            var userTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == id);
            if (userTrip == null) return Redirect("/");
            var trip = userTrip.Trip;
            ViewBag.Trip = trip;

            return View();
        }
    }
}