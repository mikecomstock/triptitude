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
            return Json(activity.Json(CurrentUser, Url), JsonRequestBehavior.AllowGet);
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

            Note note = null;
            if (!string.IsNullOrWhiteSpace(form.Note))
                note = trip.AddNote(CurrentUser, activity, form.Note);

            repo.Add(activity);
            repo.Save();

            // have to call repo.save before this so we have the id
            trip.AddHistory(CurrentUser, HistoryAction.CreateActivity, activity.Id);
            repo.Save();
            if (note != null)
            {
                trip.AddHistory(CurrentUser, HistoryAction.CreatedNote, note.Id);
                repo.Save();
            }
            return Json(activity.Json(CurrentUser, Url));
        }

        public class ActivityForm
        {
            public int TripID { get; set; }
            public string Title { get; set; }
            public DateTime? BeginAt { get; set; }
            //public DateTime? EndAt { get; set; }
            public int OrderNumber { get; set; }
            public string SourceURL { get; set; }
            public string Note { get; set; }
            public bool? Moved { get; set; }
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

            Note note = null;
            if (!string.IsNullOrWhiteSpace(form.Note))
                note = trip.AddNote(CurrentUser, activity, form.Note);

            // Only save the UpdatedActivity row when 
            // 1) They are editing the activty with the form, and there is a save button
            // 2) They drag THIS activity on the timeline. When this happends, multiple activities get save called,
            //    but only one of them gets the history.
            if (!form.Moved.HasValue || form.Moved.Value)
                trip.AddHistory(CurrentUser, HistoryAction.UpdateActivity, activity.Id);

            repo.Save();
            if (note != null)
            {
                trip.AddHistory(CurrentUser, HistoryAction.CreatedNote, note.Id);
                repo.Save();
            }
            return Json(activity.Json(CurrentUser, Url));
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