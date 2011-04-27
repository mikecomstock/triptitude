using System;
using Triptitude.Biz.Services;

namespace Triptitude.GeoNamesImporter
{
    class Program
    {
        private const string countryInfoPath = "C:\\Users\\Mike\\Desktop\\Temp\\countryInfo.txt";
        private const string allCountriesPath = "C:\\Users\\Mike\\Desktop\\Temp\\allCountries.txt";
        private const string hotelFilePath = "C:\\Source\\Datafiles\\Hotels_All.csv";

        static DateTime startTime = DateTime.Now;

        static void Main(string[] args)
        {
            //Countries();
            //Regions();
            //Cities();
            //HotelsCombined();
            //BuildSearchIndexes();

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey(false);
        }

        static void WriteTime()
        {
            Console.WriteLine((DateTime.Now - startTime).TotalMinutes + " minutes");
        }

        static void Countries()
        {
            Console.WriteLine("=== IMPORTING COUNTRIES ===");
            var importService = new ImportService();
            importService.PrepareCountries(countryInfoPath);
            WriteTime();
        }

        static void Regions()
        {
            Console.WriteLine("=== IMPORTING REGIONS ===");
            var importService = new ImportService();
            importService.PrepareRegions(allCountriesPath);
            WriteTime();
        }

        static void Cities()
        {
            Console.WriteLine("=== IMPORTING CITIES ===");
            var importService = new ImportService();
            importService.PrepareCities(allCountriesPath);
            WriteTime();
        }

        static void HotelsCombined()
        {
            Console.WriteLine("=== IMPORTING HOTELS ===");
            ImportService importService = new ImportService();
            var numErrors = importService.ImportHotelsCombinedHotels(hotelFilePath);
            if (numErrors > 0) Console.Write(String.Format("!!!!!! {0} ERRORS !!!!!!", numErrors));
            WriteTime();
        }

        private static void BuildSearchIndexes()
        {
            var luceneService = new LuceneService();
            luceneService.IndexDestinations();
            WriteTime();

            //while (true)
            //{
            //    Console.Write("Query: ");
            //    var line = Console.ReadLine();
            //    var results = luceneService.SearchDestinations(line);
            //    Console.WriteLine(results.Count());
            //    foreach (var result in results)
            //    {
            //        Console.WriteLine(result.FullName);
            //    }
            //}
        }
    }
}
