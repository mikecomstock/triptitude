namespace Triptitude.Biz.Forms
{
    public class HotelSearchForm
    {
        public string Search { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int RadiusInMiles { get; set; }
    }
}