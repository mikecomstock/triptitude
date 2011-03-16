using System;
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
    }
}