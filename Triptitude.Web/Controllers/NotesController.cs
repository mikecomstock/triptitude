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
            ViewBag.ItineraryItemId = itineraryItemId;
            return PartialView("Edit");
        }

        public ActionResult Create(NoteForm form, User currentUser)
        {
            var itineraryItem = new ItineraryItemsRepo().FindAll().First(
                    i => i.Id == form.ItineraryItemId && i.Trip.Users.Contains(currentUser));
            ItineraryItemNote note = new ItineraryItemNote
                                         {
                                             Created_By = currentUser.Id,
                                             Created_On = DateTime.UtcNow,
                                             Public = true,
                                             Text = form.Text,

                                         };
            notesRepo.Add(note);
            notesRepo.Save();
            return Redirect(Url.PublicDetails(currentUser.DefaultTrip));
        }

        public ActionResult Edit(int id)
        {
            ItineraryItemNote note = notesRepo.Find(id);
            ViewBag.Note = note;
            return PartialView();
        }

        [HttpPost]
        public ActionResult Edit(NoteForm form, User currentUser)
        {
            var note = notesRepo.Save(form);
            return Redirect(Url.PublicDetails(currentUser.DefaultTrip));
        }

        public ActionResult Delete(int id, User currentUser)
        {
            var note = notesRepo.Find(id);
            notesRepo.Delete(note);
            notesRepo.Save();
            return Redirect(Url.PublicDetails(currentUser.DefaultTrip));
        }
    }
}
