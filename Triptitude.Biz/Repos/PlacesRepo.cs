using System.Linq;
using Triptitude.Biz.Models;
using Triptitude.Biz.Services;

namespace Triptitude.Biz.Repos
{
    public class PlacesRepo : Repo<Place>
    {
        public Place FindOrInitializeByGoogReference(string googId, string googReference)
        {
            Place place = FindAll().FirstOrDefault(p => p.GoogId == googId);

            if (place == null)
            {
                place = new PlacesService().CreateFromGoogle(googReference);
            }

            return place;
        }
        public Place FindOrCreateByGoogReference(string googId, string googReference)
        {
            Place place = FindAll().FirstOrDefault(p => p.GoogId == googId);

            if (place == null)
            {
                place = new PlacesService().CreateFromGoogle(googReference);
                Add(place);
                Save();
                new Repo().ExecuteSql("update Places set GeoPoint = geography::Point(Latitude, Longitude, 4326) where Latitude is not null and Longitude is not null and Id in (@p0)", place.Id);
            }

            return place;
        }
    }
}