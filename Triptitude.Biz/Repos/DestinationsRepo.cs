using System;
using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class DestinationsRepo
    {
        public Destination Find(int id)
        {
            Country country = new CountriesRepo().Find(id);
            if (country != null) return country;

            Region region = new RegionsRepo().Find(id);
            if (region != null) return region;

            City city = new CitiesRepo().Find(id);
            if (city != null) return city;

            throw new Exception("Couldn't find destination");
        }

        public IEnumerable<Destination> Search(string term)
        {
            IEnumerable<Destination> countries = new CountriesRepo().FindAll().Where(c => c.Name.StartsWith(term)).Take(10);
            IEnumerable<Destination> regions = new RegionsRepo().FindAll().Where(c => c.Name.StartsWith(term)).Take(10);
            IEnumerable<Destination> cities = new CitiesRepo().FindAll().Where(c => c.ASCIIName.StartsWith(term)).Take(10);

            return countries.Concat(regions).Concat(cities);
        }
    }
}