using System;
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
        private NotesRepo notesRepo;
        public NotesController()
        {
            notesRepo = new NotesRepo();
        }

        public ActionResult Create(int itineraryItemId)
        {
            ItineraryItem itineraryItem = new ItineraryItemsRepo().Find(itineraryItemId);
            ViewBag.Note = new Note { ItineraryItem = itineraryItem };
            ViewBag.Action = Url.CreateNote(itineraryItemId);
            return PartialView("Form");
        }

        [HttpPost]
        public ActionResult Create(NoteForm form, User currentUser)
        {
            var itineraryItem = new ItineraryItemsRepo().Find(form.ItineraryItemId);
            bool userOwnsTrip = itineraryItem.Trip.Users.Contains(currentUser);

            if (userOwnsTrip && !string.IsNullOrWhiteSpace(form.Text))
            {
                Note note = new Note
                                             {
                                                 Created_By = currentUser.Id,
                                                 Created_On = DateTime.UtcNow,
                                                 Public = true,
                                                 Text = form.Text,
                                                 ItineraryItem = itineraryItem
                                             };
                notesRepo.Add(note);
                notesRepo.Save();
            }
            return Redirect(Url.PublicDetails(currentUser.DefaultTrip));
        }

        public ActionResult Edit(int id)
        {
            Note note = notesRepo.Find(id);
            ViewBag.Note = note;
            ViewBag.Action = Url.Edit(note);
            return PartialView("Form");
        }

        [HttpPost]
        public ActionResult Edit(NoteForm form, User currentUser)
        {
            var note = notesRepo.Find(form.Id);
            var userOwnsTrip = note.ItineraryItem.Trip.Users.Contains(currentUser);
            if (userOwnsTrip)
            {
                notesRepo.Save(form);
            }

            return Redirect(Url.PublicDetails(currentUser.DefaultTrip));
        }

        public ActionResult Delete(int id, User currentUser)
        {
            var note = notesRepo.Find(id);
            var userOwnsTrip = note.ItineraryItem.Trip.Users.Contains(currentUser);
            if (userOwnsTrip)
            {
                notesRepo.Delete(note);
                notesRepo.Save();
            }
            return Redirect(Url.PublicDetails(currentUser.DefaultTrip));
        }
    }
}
