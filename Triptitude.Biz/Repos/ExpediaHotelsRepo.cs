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
    }
}