using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Forms;
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

        public IEnumerable<Hotel> Search(HotelSearchForm form)
        {
            int radiusInMeters = (int)(form.RadiusInMiles * 1609.344);

            string sql = "select top 100 h.* from Hotels h join HotelsNear(@p0,@p1,@p2) as hn on h.Id = hn.Hotel_Id order by hn.Distance";
            var hotels = Sql(sql, form.Latitude, form.Longitude, radiusInMeters);
            return hotels;
        }
    }
}