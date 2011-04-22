using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class DestinationTagsRepo : Repo<DestinationTag>
    {
        public DestinationTag FindOrCreate(City city, Tag tag)
        {
            DestinationTag destinationTag = FindAll().FirstOrDefault(td => td.City.GeoNameID == city.GeoNameID && td.Tag.Id == tag.Id);

            if (destinationTag == null)
            {
                destinationTag = new DestinationTag { City = city, Tag = tag };
                Add(destinationTag);
                Save();
            }

            return destinationTag;
        }
    }
}