namespace Triptitude.Biz.Forms
{
    public class ActivityForm
    {
        public int? ActivityId { get; set; }
        public int TripId { get; set; }
        public int? BeginDay { get; set; }
        public int? EndDay { get; set; }
        public string TagName { get; set; }
    }

    public class TransportationActivityForm : ActivityForm
    {
        public int TransportationTypeId { get; set; }
        public int FromCityId { get; set; }
        public int ToCityId { get; set; }

        // Used only for display purposes
        public string FromCityName { get; set; }
        public string ToCityName { get; set; }
    }

    public class HotelActivityForm : ActivityForm
    {
        public int HotelId { get; set; }
    }

    public class PlaceActivityForm : ActivityForm
    {
        public int? PlaceId { get; set; }
        public string FactualId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public string Telephone { get; set; }

        public string Website { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}