namespace Triptitude.Biz.Forms
{
    public class PackingItemForm
    {
        public int? PackingItemId { get; set; }
        public int TripId { get; set; }
        public int? ActivityId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public bool Public { get; set; }
        public string TagString { get; set; }
    }
}