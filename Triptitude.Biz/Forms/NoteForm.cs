namespace Triptitude.Biz.Forms
{
    public class NoteForm
    {
        public int? NoteId { get; set; }
        public int TripId { get; set; }
        public int? ActivityId { get; set; }
        public string Text { get; set; }
        public bool Public { get; set; }
    }
}