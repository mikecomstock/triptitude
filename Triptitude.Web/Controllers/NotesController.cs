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
        private readonly NotesRepo notesRepo;

        public NotesController()
        {
            tripsRepo = new TripsRepo();
            notesRepo = new NotesRepo();
        }

        public ActionResult Create(User currentUser)
        {
            var form = new NoteForm
                           {
                               TripId = currentUser.DefaultTrip.Id,
                               Public = true
                           };
            ViewBag.Trip = currentUser.DefaultTrip;
            ViewBag.Form = form;
            ViewBag.Action = Url.CreateNote();
            return PartialView("NoteDialog");
        }

        [HttpPost]
        public ActionResult Create(NoteForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

            notesRepo.Save(form, currentUser);
            return Redirect(Url.Details(trip));
        }
    }
}