using System;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class NotesRepo : Repo<Note>
    {
        public Note Save(NoteForm form, User currentUser)
        {
            Note note;
            if (form.NoteId.HasValue)
            {
                note = Find(form.NoteId.Value);
            }
            else
            {
                note = new Note { Created_On = DateTime.UtcNow, User = currentUser };
                Add(note);
            }

            note.Text = form.Text;
            note.Public = form.Public;

            if (form.ActivityId.HasValue)
            {
                var activitiesRepo = new ActivitiesRepo();
                var activity = activitiesRepo.Find(form.ActivityId.Value);
                note.Activity = activity;
            }
            else
            {
                note.Activity = null;
            }

            Save();
            return note;
        }
    }
}