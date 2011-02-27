using System.Collections.Generic;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class RegionsRepo : Repo<Region>
    {
        public void ImportRegions(IEnumerable<Region> regions)
        {
            foreach (var region in regions)
            {
                Add(region);
                _db.SaveChanges();
            }
        }
    }

    public class CitiesRepo : Repo<City>
    {
        public void ImportCities(IEnumerable<City> cities)
        {
            foreach (var city in cities)
            {
                Add(city);
                _db.SaveChanges();
            }
        }
    }
}