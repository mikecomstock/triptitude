using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class RegionsRepo : Repo<Region>
    {
        public IQueryable<Tag> GetTags(int id)
        {
            //Find(id).
            return null;
        }
    }
}