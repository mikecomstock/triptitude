using System.Linq;
using Triptitude.Biz.Models;
using Triptitude.Biz.Services;

namespace Triptitude.Biz.Repos
{
    public class PlacesRepo : Repo<Place>
    {
        public Place FindOrInitializeByFactualId(string factualId)
        {
            Place place = FindAll().FirstOrDefault(p => p.FactualId == factualId);

            if (place == null)
            {
                place = new PlacesService().Find(factualId);
            }

            return place;
        }
    }
}