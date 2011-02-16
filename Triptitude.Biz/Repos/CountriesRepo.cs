using System.Collections.Generic;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class CountriesRepo : Repo<Country>
    {
        public void ImportCountries(IEnumerable<Country> countries)
        {
            foreach (var country in countries)
            {
                Add(country);
                _db.SaveChanges();
            }
        }
    }
}