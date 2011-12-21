﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class TripsController : Controller
    {
        // partial only
        //public ActionResult SidePanel(Trip trip, User currentUser)
        //{
        //    ViewBag.Trip = trip;
        //    ViewBag.UserOwnsTrip = currentUser.OwnsTrips(trip);
        //    return PartialView("_SidePanel");
        //}

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
            var tripSearchForm = new TripSearchForm();
            ViewBag.TripSearchForm = tripSearchForm;
            return View();
        }

        public ActionResult SearchResults(TripSearchForm form)
        {
            ViewBag.Trips = form.Results;
            return PartialView();
        }

        public ActionResult Print(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            return View();
        }

        public ActionResult Map(int id)
        {
            Trip trip = new TripsRepo().Find(id);
            return RedirectPermanent(Url.Details(trip) + "#map");
            //ViewBag.Trip = trip;
            //return View();
        }

        public ActionResult Details(int id, User currentUser)
        {
            Trip trip = new TripsRepo().Find(id);
            if (trip == null) return HttpNotFound();

            ViewBag.Trip = trip;
            ViewBag.CurrentUser = currentUser;
            return View();
        }

        // partial only
        public ActionResult DayDetails(Trip trip, int? dayNumber, User currentUser)
        {
            // For unscheduled activities
            dayNumber = dayNumber == -1 ? null : dayNumber;

            ViewBag.DayNumber = dayNumber;
            ViewBag.Trip = trip;
            ViewBag.Editing = currentUser != null && currentUser.DefaultTrip == trip;
            ViewBag.CurrentUserOwnsTrip = currentUser.OwnsTrips(trip);
            ViewBag.CurrentUser = currentUser;
            return PartialView("_DayDetails");
        }

        public ActionResult PackingList(int id, User currentUser)
        {
            Trip trip = new TripsRepo().Find(id);
            return RedirectPermanent(Url.Details(trip) + "#packinglist");
        }

        public ActionResult PackingListPartial(int id, User currentUser)
        {
            Trip trip = new TripsRepo().Find(id);
            ViewBag.Trip = trip;
            bool userOwnsTrip = currentUser.OwnsTrips(trip);
            ViewBag.UserOwnsTrip = userOwnsTrip;
            ViewBag.CurrentUser = currentUser;

            var packingListItems = trip.PackingListItems.Where(pli => userOwnsTrip || pli.Visibility == Visibility.Public).OrderBy(pli => pli.ItemTag.Item.Name);
            ViewBag.PackingListItems = packingListItems;
            var tags = packingListItems.Select(pli => pli.ItemTag).Select(it => it.Tag).Where(t => t != null).Distinct();
            ViewBag.Tags = tags;

            var itemIdsUsed = packingListItems.Select(pli => pli.ItemTag.Item.Id).Distinct();
            ViewBag.Suggestions = new ItemTagRepo().MostPopular().Where(it => !itemIdsUsed.Contains(it.Item.Id)).Take(9);
            return PartialView("PackingList");
        }

        public ActionResult Settings(int id, User currentUser)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);
            if (!currentUser.OwnsTrips(trip)) return Redirect("/");

            ViewBag.Trip = trip;
            ViewBag.Form = tripRepo.GetSettingsForm(trip);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(int id, TripSettingsForm form, User currentUser)
        {
            var tripRepo = new TripsRepo();
            Trip trip = tripRepo.Find(id);
            if (!currentUser.OwnsTrips(trip)) return Redirect("/");

            if (ModelState.IsValid)
            {
                tripRepo.Save(trip, form);
                return Redirect(Url.Details(trip));
            }
            else
            {
                ViewBag.Trip = trip;
                ViewBag.Form = form;
                return View();
            }
        }

        public ActionResult Create(int? to, User currentUser)
        {
            var form = new CreateTripForm();

            if (to.HasValue)
            {
                var placesRepo = new PlacesRepo();
                Place place = placesRepo.Find(to.Value);
                form.ToGoogReference = place.GoogReference;
                form.ToGoogId = place.GoogId;
                form.ToName = place.Name;
            }

            if (to.HasValue)
            {
                return Create(form, currentUser);
            }
            else
            {
                ViewBag.Form = form;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateTripForm form, User currentUser)
        {
            if (ModelState.IsValid)
            {
                Trip trip = new TripsRepo().Save(form, currentUser);
                new UsersRepo().SetDefaultTrip(currentUser, trip);
                return Redirect(Url.Details(trip));
            }
            else
            {
                ViewBag.Form = form;
                return View();
            }
        }
    }
}