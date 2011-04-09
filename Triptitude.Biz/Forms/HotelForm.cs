namespace Triptitude.Biz.Forms
{
    public class HotelForm
    {
        public int? ItineraryItemId { get; set; }
        public int TripId { get; set; }
        public int HotelId { get; set; }
        public int? BeginDay { get; set; }
        public int? EndDay { get; set; }
    }
}