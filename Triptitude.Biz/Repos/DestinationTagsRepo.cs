using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class DestinationTagsRepo : Repo<TagDestination>
    {
        public TagDestination FindOrCreate(City city, Tag tag)
        {
            TagDestination tagDestination = FindAll().FirstOrDefault(td => td.City.GeoNameID == city.GeoNameID && td.Tag.Id == tag.Id);

            if (tagDestination == null)
            {
                tagDestination = new TagDestination { City = city, Tag = tag };
                Add(tagDestination);
                Save();
            }

            return tagDestination;
        }
    }
}