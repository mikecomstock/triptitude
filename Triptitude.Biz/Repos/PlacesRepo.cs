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
            }

            return place;
        }
    }
}