using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class ExpediaHotelsRepo : Repo<ExpediaHotel>
    {
        public IQueryable<ExpediaHotel> Search(string s)
        {
            IQueryable<ExpediaHotel> hotels = FindAll();
            if (!string.IsNullOrWhiteSpace(s))
                hotels = hotels.Where(h => h.BaseItem.Name.Contains(s));

            return hotels;
        }

        public ExpediaHotel FindByBaseItemId(int id)
        {
            return FindAll().FirstOrDefault(h => h.BaseItem.Id == id);
        }

        public IEnumerable<ExpediaHotel> FindNear(City city, int radiusInMeters)
        {
            const string sql = "select eh.* from ExpediaHotels eh join BaseItemsNear(@p0,@p1,@p2) as bin on eh.BaseItem_Id = bin.Id order by bin.Distance";
            var hotels = Sql(sql, city.Latitude, city.Longitude, radiusInMeters);
            return hotels;
        }
    }
}