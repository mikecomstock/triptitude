using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class HotelsRepo : Repo<Hotel>
    {
        public IQueryable<Hotel> Search(string s)
        {
            IQueryable<Hotel> hotels = FindAll();
            if (!string.IsNullOrWhiteSpace(s))
                hotels = hotels.Where(h => h.Name.Contains(s));

            return hotels;
        }

        public IEnumerable<Hotel> FindNear(City city, int radiusInMeters)
        {
            const string sql = "select h.* from Hotels h join HotelsNear(@p0,@p1,@p2) as hn on h.Id = hn.Id order by hn.Distance";
            var hotels = Sql(sql, city.Latitude, city.Longitude, radiusInMeters);
            return hotels;
        }
    }
}