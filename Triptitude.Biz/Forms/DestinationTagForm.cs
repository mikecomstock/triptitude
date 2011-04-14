namespace Triptitude.Biz.Forms
{
    public class DestinationTagForm
    {
        public int? ItineraryItemId { get; set; }
        public int TripId { get; set; }
        public int? BeginDay { get; set; }
        public int? EndDay { get; set; }
        public string TagName { get; set; }
        public int DestinationId { get; set; }

        // Used only for display purposes?
        public string DestinationName { get; set; }
    }
}