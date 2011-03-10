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

        private const string hotelsAllActivePath = "C:\\Users\\Mike\\Desktop\\Temp\\Hotel_All_Active 02-26-11.txt";
        private const string hotelsOutPath = "C:\\Users\\Mike\\Desktop\\Temp\\Hotels.sql";

        private const string hotelImagesPath = "C:\\Users\\Mike\\Desktop\\Temp\\images.txt";
        private const string hotelImagesOutPath = "C:\\Users\\Mike\\Desktop\\Temp\\Images.sql";

        static void Main(string[] args)
        {
            //Countries();
            //Regions();
            //Cities();
            //Hotels();
            HotelImages();

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey(false);
        }

        static void Countries()
        {
            Console.WriteLine("=== PERPARING COUNTRIES ===");
            var importService = new ImportService();
            importService.PrepareCountries(countryInfoPath, countriesOutPath);
        }

        static void Regions()
        {
            Console.WriteLine("=== PERPARING REGIONS ===");
            var importService = new ImportService();
            importService.PrepareRegions(allCountriesPath, regionsOutPath);
        }

        static void Cities()
        {
            Console.WriteLine("=== PERPARING CITIES ===");
            var importService = new ImportService();
            importService.PrepareCities(allCountriesPath, citiesOutPath);
        }

        static void Hotels()
        {
            Console.WriteLine("=== IMPORTING HOTELS ===");
            ImportService importService = new ImportService();
            importService.ImportHotels(hotelsAllActivePath, hotelsOutPath);
        }

        static void HotelImages()
        {
            Console.WriteLine("=== IMPORTING HOTEL IMAGES ===");
            ImportService importService = new ImportService();
            importService.ImportHotelImages(hotelImagesPath, hotelImagesOutPath);
        }
    }
}
