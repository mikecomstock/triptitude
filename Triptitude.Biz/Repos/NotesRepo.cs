using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class NotesRepo : Repo<ItineraryItemNote>
    {
        public ItineraryItemNote Save(NoteForm form)
        {
            var note = Find(form.Id);
            note.Text = form.Text;

            if (string.IsNullOrWhiteSpace(note.Text))
            {
                Delete(note);
            }
            Save();

            // Should his return null if the note was deleted?
            return note;
        }
    }
}