namespace Triptitude.Biz.Forms
{
    public class WebsiteForm
    {
        public int? ItineraryItemId { get; set; }
        public int TripId { get; set; }
        public int? BeginDay { get; set; }
        public int? EndDay { get; set; }
        public string Url { get; set; }
    }
}