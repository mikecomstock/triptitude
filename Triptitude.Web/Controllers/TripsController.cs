using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;

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
                return Json(trip.Json(CurrentUser, Url), JsonRequestBehavior.AllowGet);

            ViewBag.Trip = trip;
            return View();
        }

        public ActionResult Itinerary(int id)
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
                return Json(trip.Json(CurrentUser, Url), JsonRequestBehavior.AllowGet);

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
        //public ActionResult DayDetails(Trip trip, int? dayNumber)
        //{
        //    // For unscheduled activities
        //    dayNumber = dayNumber == -1 ? null : dayNumber;

        //    ViewBag.DayNumber = dayNumber;
        //    ViewBag.Trip = trip;
        //    ViewBag.Editing = CurrentUser.DefaultTrip == trip;
        //    ViewBag.CurrentUser = CurrentUser;
        //    return PartialView("_DayDetails");
        //}

        public ActionResult Packing(int id)
        {
            var userTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == id);
            if (userTrip == null) return Redirect("/");
            ViewBag.Trip = userTrip.Trip;

            return View();
        }

        //public ActionResult PackingList(int id)
        //{
        //    Trip trip = new TripsRepo().Find(id);
        //    return RedirectPermanent(Url.Details(trip) + "#packinglist");
        //}

        //public ActionResult PackingListPartial(int id)
        //{
        //    Trip trip = new TripsRepo().Find(id);
        //    ViewBag.Trip = trip;
        //    bool userOwnsTrip = CurrentUser.OwnsTrips(trip);
        //    ViewBag.UserOwnsTrip = userOwnsTrip;
        //    ViewBag.CurrentUser = CurrentUser;

        //    var packingListItems = trip.PackingListItems.Where(pli => userOwnsTrip || pli.Visibility == Visibility.Public).OrderBy(pli => pli.ItemTag.Item.Name);
        //    ViewBag.PackingListItems = packingListItems;
        //    var tags = packingListItems.Select(pli => pli.ItemTag).Select(it => it.Tag).Where(t => t != null).Distinct();
        //    ViewBag.Tags = tags;

        //    var itemIdsUsed = packingListItems.Select(pli => pli.ItemTag.Item.Id).Distinct();
        //    ViewBag.Suggestions = new ItemTagRepo().MostPopular().Where(it => !itemIdsUsed.Contains(it.Item.Id)).Take(9);
        //    return PartialView("PackingList");
        //}

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
            trip.AddHistory(CurrentUser, HistoryAction.UpdatedTrip);
            repo.Save();
            TempData["saved"] = true;
            return Redirect(Url.Settings(trip));
        }

        //public ActionResult Create(int? to)
        public ActionResult Create()
        {
            return Redirect("/trips/wizard");

            var form = new NewCreateTripForm { Visibility = UserTrip.UserTripVisibility.Public };

            //if (to.HasValue)
            //{
            //    var placesRepo = new PlacesRepo();
            //    Place place = placesRepo.Find(to.Value);
            //    form.Name = "Trip to " + place.Name;
            //}

            ViewBag.Form = form;
            return View();
        }

        [HttpPost]
        public ActionResult Create(NewCreateTripForm form)
        {
            if (!string.IsNullOrWhiteSpace(form.Destination))
                form.Name = "My trip to " + form.Destination.Trim().ToTitleCase();

            if (ModelState.IsValid)
            {
                Trip trip = new Trip
                {
                    Name = form.Name,
                    Created_On = DateTime.UtcNow,
                    ModifiedUTC = DateTime.UtcNow,
                };

                repo.Add(trip);

                UserTrip userTrip = new UserTrip
                {
                    Trip = trip,
                    IsCreator = true,
                    Created_On = DateTime.UtcNow,
                    UpToDateAsOfUTC = DateTime.UtcNow,
                    User = CurrentUser,
                    Visibility = (byte)form.Visibility
                };

                trip.UserTrips.Add(userTrip);
                repo.Save();

                // weird errors unless repo.save is called before setting default trip
                CurrentUser.DefaultTrip = trip;
                trip.AddHistory(CurrentUser, HistoryAction.CreatedTrip);
                repo.Save();

                EmailService.SendTripCreated(trip, Url);
                return Redirect(Url.Details(trip));
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

            trip.AddHistory(CurrentUser, HistoryAction.DeletedTrip);
            repo.Save();

            return Json(trip.Json(CurrentUser, Url));
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

        [HttpGet]
        public ActionResult Wizard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Wizard(TripWizard form)
        {
            var tripTitle = string.IsNullOrWhiteSpace(form.Title) ? "My trip into the unknown" : form.Title;
            Trip trip = new Trip
            {
                Name = tripTitle,
                Created_On = DateTime.UtcNow,
                ModifiedUTC = DateTime.UtcNow,
            };
            repo.Add(trip);

            {
                UserTrip userTrip = new UserTrip
                                        {
                                            Trip = trip,
                                            IsCreator = true,
                                            Created_On = DateTime.UtcNow,
                                            UpToDateAsOfUTC = DateTime.UtcNow,
                                            User = CurrentUser,
                                            Visibility =
                                                form.Visibility == "Public"
                                                    ? (byte)UserTrip.UserTripVisibility.Public
                                                    : (byte)UserTrip.UserTripVisibility.Private
                                        };
                trip.UserTrips.Add(userTrip);
            }

            foreach (var traveler in form.Travelers)
            {
                if (string.IsNullOrWhiteSpace(traveler)) continue;

                UserTrip userTrip = new UserTrip
                {
                    Trip = trip,
                    Created_On = DateTime.UtcNow,
                    UpToDateAsOfUTC = DateTime.UtcNow,
                    User = new User { Name = traveler },
                    Visibility = (byte)UserTrip.UserTripVisibility.Private
                };
                trip.UserTrips.Add(userTrip);
            }

            var order = 1;

            // Transportation there
            {
                var title = form.How;
                title += string.IsNullOrWhiteSpace(form.From) ? string.Empty : " from " + form.From;
                title += string.IsNullOrWhiteSpace(form.To) ? " to your destination" : " to " + form.To;
                Activity a = new Activity
                                 {
                                     Title = title,
                                     BeginAt = form.Begin.HasValue ? form.Begin : null,
                                     OrderNumber = order++
                                 };
                trip.Activities.Add(a);
            }

            // Check in to hotel
            trip.Activities.Add(new Activity
                                        {
                                            Title = "Check in to hotel",
                                            BeginAt = form.Begin.HasValue ? form.Begin : null,
                                            OrderNumber = order++
                                        });

            // Explore
            trip.Activities.Add(new Activity
                                    {
                                        Title = "Begin exploring " + (string.IsNullOrWhiteSpace(form.To) ? "your destination" : form.To),
                                        BeginAt = form.Begin.HasValue ? form.Begin.Value.AddDays(1) : (DateTime?)null,
                                        OrderNumber = order++
                                    });

            // Check out of hotel
            trip.Activities.Add(new Activity
                {
                    Title = "Check out of hotel",
                    BeginAt = form.End.HasValue ? form.End : null,
                    OrderNumber = order++
                });

            // Transportation back home
            {
                var title = form.How;
                title += string.IsNullOrWhiteSpace(form.To) ? string.Empty : " from " + form.To;
                title += string.IsNullOrWhiteSpace(form.From) ? " back to home" : " back to " + form.From;
                Activity a = new Activity
                                 {
                                     Title = title,
                                     BeginAt = form.End.HasValue ? form.End : null,
                                     OrderNumber = order++
                                 };
                trip.Activities.Add(a);
            }

            if (form.Packing)
            {
                var numDays = 1;
                if (form.Begin.HasValue && form.End.HasValue)
                    numDays = (form.End.Value - form.Begin.Value).Days;

                trip.PackingItems.Add(new PackingItem { Item = "Toothbrush", Tag = "Hygiene", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Toothpaste", Tag = "Hygiene", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Deodorant", Tag = "Hygiene", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Soap", Tag = "Hygiene", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Shampoo", Tag = "Hygiene", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Medications", Tag = "Hygiene", Quantity = 1 });

                trip.PackingItems.Add(new PackingItem { Item = "Underwear", Tag = "Clothing", Quantity = 1 * numDays });
                trip.PackingItems.Add(new PackingItem { Item = "Socks", Tag = "Clothing", Quantity = 1 * numDays });
                trip.PackingItems.Add(new PackingItem { Item = "Sleapwear", Tag = "Clothing", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Sandals/Flip Flops", Tag = "Clothing", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Belt", Tag = "Clothing", Quantity = 1 });

                trip.PackingItems.Add(new PackingItem { Item = "Jacket", Tag = "Outdoors", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Sunscreen", Tag = "Outdoors", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Sunglasses", Tag = "Outdoors", Quantity = 1 });

                trip.PackingItems.Add(new PackingItem { Item = "Passport", Tag = "Documents", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Photo ID", Tag = "Documents", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Reservation Info", Tag = "Documents", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Itinerary", Tag = "Documents", Quantity = 1 });

                trip.PackingItems.Add(new PackingItem { Item = "Backpack", Tag = "Misc", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Umbrella", Tag = "Misc", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Pleasure Reading", Tag = "Misc", Quantity = 1 });

                trip.PackingItems.Add(new PackingItem { Item = "Phone & Charger", Tag = "Electronics", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Camera", Tag = "Electronics", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Laptop & Charger", Tag = "Electronics", Quantity = 1 });
                trip.PackingItems.Add(new PackingItem { Item = "Headphones", Tag = "Electronics", Quantity = 1 });

                foreach (var packingItem in trip.PackingItems)
                {
                    packingItem.CreatedUser = CurrentUser;
                    packingItem.ForUser = CurrentUser;
                    packingItem.Created_At = DateTime.UtcNow;
                    packingItem.Modified_At = DateTime.UtcNow;
                }
            }

            repo.Save();

            // weird errors unless repo.save is called before setting default trip
            CurrentUser.DefaultTrip = trip;
            trip.AddHistory(CurrentUser, HistoryAction.CreatedTrip);
            repo.Save();

            EmailService.SendTripCreated(trip, Url);
            return Redirect(Url.Details(trip));
        }

        public class TripWizard
        {
            public string To { get; set; }
            public string From { get; set; }
            public string How { get; set; }
            public string Title { get; set; }
            public string Visibility { get; set; }
            public DateTime? Begin { get; set; }
            public DateTime? End { get; set; }
            public List<string> Travelers { get; set; }
            public bool Packing { get; set; }
        }
    }
}