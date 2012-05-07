using System;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class ActivitiesController : TriptitudeController
    {
        private readonly TripsRepo tripsRepo;
        private readonly ActivitiesRepo repo;

        public ActivitiesController()
        {
            tripsRepo = new TripsRepo();
            repo = new ActivitiesRepo();
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

            repo.Add(activity);
            repo.Save();

            // have to call repo.save before this so we have the id
            trip.AddHistory(CurrentUser, HistoryAction.CreateActivity, activity.Id);
            repo.Save();

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
            var activity = repo.Find(id);
            var trip = activity.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            activity.Title = form.Title;
            activity.BeginAt = form.BeginAt.HasValue ? form.BeginAt.Value.ToUniversalTime() : (DateTime?)null;
            //activity.EndAt = form.EndAt.HasValue ? DateTime.SpecifyKind(form.EndAt.Value, DateTimeKind.Utc) : (DateTime?)null;
            activity.OrderNumber = form.OrderNumber;
            activity.SourceURL = form.SourceURL;

            trip.AddHistory(CurrentUser, HistoryAction.UpdateActivity, activity.Id);

            repo.Save();
            return Json(activity.Json(CurrentUser));
        }


        public ActionResult Delete(int id)
        {
            var activity = repo.Find(id);
            var trip = activity.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            activity.Deleted = true;
            activity.OrderNumber = 0;

            trip.AddHistory(CurrentUser, HistoryAction.DeletedActivity, activity.Id);

            repo.Save();

            return Content("success");
        }
    }
}