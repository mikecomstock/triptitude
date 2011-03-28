namespace Triptitude.Biz.Forms
{
    public class TransportationForm
    {
        public int? Id { get; set; }
        public int TripId { get; set; }
        public int FromCityId { get; set; }
        public int ToCityId { get; set; }
        public int? BeginDay { get; set; }
        public int? EndDay { get; set; }

        // Used only for display purposes
        public string FromCityName { get; set; }
        public string ToCityName { get; set; }
    }
}