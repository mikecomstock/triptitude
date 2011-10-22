namespace Triptitude.Biz.Forms
{
    public class PlaceSearchForm
    {
        public string Search { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int RadiusInMiles { get; set; }

        public double RadiusInKm { get { return RadiusInMiles * 1609.344; } }
    }
}