using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class DestinationTagsRepo : Repo<DestinationTag>
    {
        public DestinationTag FindOrCreate(Destination destination, Tag tag)
        {
            DestinationTag destinationTag = FindAll().FirstOrDefault(td => td.Destination.Id == destination.Id && td.Tag.Id == tag.Id);

            if (destinationTag == null)
            {
                destinationTag = new DestinationTag { Destination = destination, Tag = tag };
                Add(destinationTag);
                Save();
            }

            return destinationTag;
        }
    }
}