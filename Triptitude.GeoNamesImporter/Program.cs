using System;
using System.Collections.Generic;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;

namespace Triptitude.GeoNamesImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            GeoNamesService geoNamesService = new GeoNamesService();
            CountriesRepo countriesRepo = new CountriesRepo();

            IEnumerable<Country> countries = geoNamesService.GetCountries();

            foreach (var country in countries)
            {
                Console.WriteLine("{0}\t{1}", country.ISO, country.Name);
            }

            countriesRepo.ImportCountries(countries);

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey(false);
        }
    }
}
