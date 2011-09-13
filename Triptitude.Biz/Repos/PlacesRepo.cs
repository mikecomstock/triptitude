using System.Linq;
using Triptitude.Biz.Models;
using Triptitude.Biz.Services;

namespace Triptitude.Biz.Repos
{
    public class PlacesRepo : Repo<Place>
    {
        public Place FindOrInitializeByGoogReference(string googReference)
        {
            Place place = FindAll().FirstOrDefault(p => p.GoogReference == googReference);

            if (place == null)
            {
                place = new PlacesService().FindGoogle(googReference);
            }

            return place;
        }
    }
}