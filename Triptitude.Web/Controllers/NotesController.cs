using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class NotesController : Controller
    {
        private readonly TripsRepo tripsRepo;
        //private readonly NotesRepo notesRepo;

        public NotesController()
        {
            tripsRepo = new TripsRepo();
            //notesRepo = new NotesRepo();
        }

        //public ActionResult _NotesFor(User currentUser, int activityId)
        //{
        //    var activitiesRepo = new ActivitiesRepo();
        //    Activity activity = activitiesRepo.Find(activityId);

        //    var trip = activity.Trip;
        //    bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
        //    if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

        //    var notes = activity.Notes.OrderBy(n => n.Id);
        //    ViewBag.Notes = notes;
        //    return PartialView("_NotesFor");
        //}

        public ActionResult Create(User currentUser)
        {
            var form = new NoteForm
                           {
                               TripId = currentUser.DefaultTrip.Id,
                               ActivityId = currentUser.DefaultTrip.Activities.First().Id,
                               Public = true
                           };
            ViewBag.Form = form;
            ViewBag.Trip = currentUser.DefaultTrip;
            //ViewBag.Notes = currentUser.DefaultTrip.Activities.SelectMany(a => a.Notes).OrderBy(n => n.Id);
            ViewBag.Action = string.Empty;// Url.CreateNote();
            return PartialView("NoteDialog");
        }

        //[HttpPost]
        //public ActionResult Create(NoteForm form, User currentUser)
        //{
        //    var trip = tripsRepo.Find(form.TripId);
        //    bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
        //    if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

        //    notesRepo.Save(form, currentUser);
        //    return Redirect(Url.Details(trip));
        //}
    }
}