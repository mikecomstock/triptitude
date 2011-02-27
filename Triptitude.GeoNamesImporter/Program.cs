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
           // Countries();
          //  Regions();
            Cities();

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey(false);
        }

        static void Countries()
        {
            Console.WriteLine("=== IMPORTING COUNTRIES ===");
            GeoNamesService geoNamesService = new GeoNamesService();
            IEnumerable<Country> countries = geoNamesService.GetCountries();

            //foreach (var country in countries)
            //{
            //    Console.WriteLine("{0}\t{1}", country.ISO, country.Name);
            //}

            CountriesRepo countriesRepo = new CountriesRepo();
            countriesRepo.ImportCountries(countries);
        }

        static void Regions()
        {
            Console.WriteLine("=== IMPORTING REGIONS ===");
            GeoNamesService geoNamesService = new GeoNamesService();
            IEnumerable<Region> regions = geoNamesService.GetAdmin1Regions();

            RegionsRepo regionsRepo = new RegionsRepo();
            //foreach (var region in regions)
            //{
            //    Console.WriteLine("{0}\t{1}", region.GeoNameAdmin1Code, region.ASCIIName);
            //    regionsRepo.ImportRegions(new[] { region });
            //}

            regionsRepo.ImportRegions(regions);
        }

        static void Cities()
        {
            Console.WriteLine("=== IMPORTING CITIES ===");
            GeoNamesService geoNamesService = new GeoNamesService();
            geoNamesService.GetCities();
        }
    }
}
