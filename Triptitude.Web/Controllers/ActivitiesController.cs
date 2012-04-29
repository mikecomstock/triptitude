using System;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class ActivitiesController : TriptitudeController
    {
        private readonly TripsRepo tripsRepo;
        private readonly ActivitiesRepo activitiesRepo;
        private readonly ActivitiesRepo repo;
        private readonly TransportationTypesRepo transportationTypesRepo;

        public ActivitiesController()
        {
            tripsRepo = new TripsRepo();
            activitiesRepo = new ActivitiesRepo();
            repo = new ActivitiesRepo();
            transportationTypesRepo = new TransportationTypesRepo();
        }

        public JsonResult Details(int id)
        {
            var activity = repo.Find(id);
            return Json(activity.Json(CurrentUser), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(ActivityForm form)
        {
            var trip = tripsRepo.Find(form.TripID);
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            Activity activity = new Activity { Trip = trip };
            activity.Title = form.Title;
            activity.BeginAt = form.BeginAt.HasValue ? form.BeginAt.Value.ToUniversalTime() : (DateTime?)null;
            //activity.EndAt = form.EndAt.HasValue ? DateTime.SpecifyKind(form.EndAt.Value, DateTimeKind.Utc) : (DateTime?)null;
            activity.OrderNumber = form.OrderNumber;
            activity.SourceURL = form.SourceURL;

            activitiesRepo.Add(activity);

            activitiesRepo.Save();
            return Json(activity.Json(CurrentUser));
        }

        public class ActivityForm
        {
            public int TripID { get; set; }
            public string Title { get; set; }
            public DateTime? BeginAt { get; set; }
            //public DateTime? EndAt { get; set; }
            public int OrderNumber { get; set; }
            public string SourceURL { get; set; }
        }

        [HttpPut]
        public ActionResult Update(int id, ActivityForm form)
        {
            var activity = activitiesRepo.Find(id);
            var trip = activity.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            activity.Title = form.Title;
            activity.BeginAt = form.BeginAt.HasValue ? form.BeginAt.Value.ToUniversalTime() : (DateTime?)null;
            //activity.EndAt = form.EndAt.HasValue ? DateTime.SpecifyKind(form.EndAt.Value, DateTimeKind.Utc) : (DateTime?)null;
            activity.OrderNumber = form.OrderNumber;
            activity.SourceURL = form.SourceURL;

            activitiesRepo.Save();
            return Json(activity.Json(CurrentUser));
        }


        public ActionResult Delete(int id)
        {
            var activity = activitiesRepo.Find(id);
            var trip = activity.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            activity.Deleted = true;
            activity.OrderNumber = 0;
            activitiesRepo.Save();

            new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Deleted, HistoryTable.Activities, activity.Id);
            return Content("success");
        }

        //public ActionResult Edit(int id, ActivityForm.Tabs selectedTab = ActivityForm.Tabs.Details)
        //{
        //    var activity = activitiesRepo.Find(id);
        //    var trip = activity.Trip;
        //    if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

        //    if (activity is TransportationActivity) return EditTransportation(activity as TransportationActivity, selectedTab);
        //    if (activity is PlaceActivity) return EditPlace(activity as PlaceActivity, selectedTab);

        //    throw new Exception("Activity type not supported");
        //}

        //public ActionResult Create(string type, int? placeId)
        //{
        //    switch (type)
        //    {
        //        case "transportation": return AddTransportation();
        //        case "place": return AddPlace(placeId);
        //    }

        //    throw new Exception("Activity type not supported");
        //}

        #region Transportation

        //private ActionResult AddTransportation()
        //{
        //    var fly = transportationTypesRepo.FindAll().First(tt => tt.Name == "Fly");
        //    TransportationActivityForm form = new TransportationActivityForm { TripId = CurrentUser.DefaultTrip.Id, TransportationTypeId = fly.Id };
        //    ViewBag.Form = form;
        //    ViewBag.TransportationTypes = transportationTypesRepo.FindAll().OrderBy(t => t.Name);
        //    ViewBag.Action = Url.ItineraryAddTransportation();
        //    return PartialView("TransportationDialog");
        //}

        //[HttpPost]
        //public ActionResult AddTransportation(TransportationActivityForm form)
        //{
        //    var trip = tripsRepo.Find(form.TripId);
        //    if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

        //    Activity activity = activitiesRepo.Save(form, CurrentUser);
        //    new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Created, HistoryTable.Activities, activity.Id);

        //    var response = new { status = "OK" };
        //    return Json(response);
        //}

        //private ActionResult EditTransportation(TransportationActivity activity, ActivityForm.Tabs selectedTab)
        //{
        //    TransportationActivityForm form = new TransportationActivityForm();
        //    form.SetBaseProps(activity);
        //    form.TransportationTypeId = activity.TransportationType == null ? (int?)null : activity.TransportationType.Id;
        //    form.SelectedTab = selectedTab;

        //    if (activity.FromPlace != null)
        //    {
        //        form.FromName = activity.FromPlace.Name;
        //        form.FromGoogReference = activity.FromPlace.GoogReference;
        //        form.FromGoogId = activity.FromPlace.GoogId;
        //    }
        //    if (activity.ToPlace != null)
        //    {
        //        form.ToName = activity.ToPlace.Name;
        //        form.ToGoogReference = activity.ToPlace.GoogReference;
        //        form.ToGoogId = activity.ToPlace.GoogId;
        //    }

        //    ViewBag.Form = form;
        //    ViewBag.TransportationTypes = transportationTypesRepo.FindAll().OrderBy(t => t.Name);
        //    ViewBag.Action = Url.ItineraryEditTransportation();
        //    return PartialView("TransportationDialog");
        //}

        //[HttpPost]
        //public ActionResult EditTransportation(TransportationActivityForm form)
        //{
        //    var activity = (TransportationActivity)activitiesRepo.Find(form.ActivityId.Value);
        //    var oldTrip = activity.Trip;
        //    var newTrip = tripsRepo.Find(form.TripId);
        //    if (!CurrentUser.OwnsTrips(oldTrip, newTrip)) return Redirect("/");

        //    activitiesRepo.Save(form, CurrentUser);
        //    new HistoriesRepo().Create(CurrentUser, newTrip, HistoryAction.Modified, HistoryTable.Activities, activity.Id);
        //    var response = new { status = "OK" };
        //    return Json(response);
        //}

        #endregion

        #region Places

        //private ActionResult AddPlace(int? placeId)
        //{
        //    Place place;

        //    if (placeId.HasValue)
        //    {
        //        var placesRepo = new PlacesRepo();
        //        place = placesRepo.Find(placeId.Value);
        //    }
        //    else
        //    {
        //        place = new Place();
        //    }

        //    PlaceActivityForm form = new PlaceActivityForm
        //    {
        //        Name = place.Name,
        //        GoogReference = place.GoogReference,
        //        GoogId = place.GoogId,
        //        TripId = CurrentUser.DefaultTrip.Id
        //    };
        //    ViewBag.Form = form;
        //    ViewBag.Place = place;
        //    ViewBag.Action = Url.ItineraryAddPlace();
        //    return PartialView("PlaceDialog");
        //}

        //[HttpPost]
        //public ActionResult AddPlace(PlaceActivityForm form)
        //{
        //    var trip = tripsRepo.Find(form.TripId);
        //    if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

        //    Activity activity = activitiesRepo.Save(form, CurrentUser);
        //    new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Created, HistoryTable.Activities, activity.Id);

        //    var response = new { status = "OK" };
        //    return Json(response);
        //}

        //private ActionResult EditPlace(PlaceActivity activity, ActivityForm.Tabs selectedTab)
        //{
        //    var form = new PlaceActivityForm();
        //    form.SetBaseProps(activity);
        //    form.SelectedTab = selectedTab;

        //    if (activity.Place != null)
        //    {
        //        form.Name = activity.Place.Name;
        //        form.GoogReference = activity.Place.GoogReference;
        //        form.GoogId = activity.Place.GoogId;
        //    }

        //    ViewBag.Form = form;
        //    ViewBag.Place = activity.Place;
        //    ViewBag.Action = Url.ItineraryEditPlace();
        //    return PartialView("PlaceDialog");
        //}

        //[HttpPost]
        //public ActionResult EditPlace(PlaceActivityForm form)
        //{
        //    PlaceActivity activity = (PlaceActivity)activitiesRepo.Find(form.ActivityId.Value);
        //    var oldTrip = activity.Trip;
        //    var newTrip = tripsRepo.Find(form.TripId);
        //    if (!CurrentUser.OwnsTrips(oldTrip, newTrip)) return Redirect("/");

        //    activitiesRepo.Save(form, CurrentUser);
        //    new HistoriesRepo().Create(CurrentUser, oldTrip, HistoryAction.Modified, HistoryTable.Activities, activity.Id);

        //    var response = new { status = "OK" };
        //    return Json(response);
        //}

        #endregion
    }
}