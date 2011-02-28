using System;
using Triptitude.Biz.Services;

namespace Triptitude.GeoNamesImporter
{
    class Program
    {
        private const string countryInfoPath = "C:\\Users\\Mike\\Desktop\\Temp\\countryInfo.txt";
        private const string allCountriesPath = "C:\\Users\\Mike\\Desktop\\Temp\\allCountries.txt";

        private const string countriesOutPath = "C:\\Users\\Mike\\Desktop\\Temp\\Countries.txt";
        private const string regionsOutPath = "C:\\Users\\Mike\\Desktop\\Temp\\Regions.txt";
        private const string citiesOutPath = "C:\\Users\\Mike\\Desktop\\Temp\\Cities.txt";

        static void Main(string[] args)
        {
            Countries();
            Regions();
            Cities();

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey(false);
        }

        static void Countries()
        {
            Console.WriteLine("=== PERPARING COUNTRIES ===");
            GeoNamesService geoNamesService = new GeoNamesService();
            geoNamesService.PrepareCountries(countryInfoPath, countriesOutPath);
        }

        static void Regions()
        {
            Console.WriteLine("=== PERPARING REGIONS ===");
            GeoNamesService geoNamesService = new GeoNamesService();
            geoNamesService.PrepareRegions(allCountriesPath, regionsOutPath);
        }

        static void Cities()
        {
            Console.WriteLine("=== PERPARING CITIES ===");
            GeoNamesService geoNamesService = new GeoNamesService();
            geoNamesService.PrepareCities(allCountriesPath, citiesOutPath);
        }
    }
}
