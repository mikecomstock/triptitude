using System;
using System.Linq;
using Triptitude.Biz.Services;

namespace Triptitude.GeoNamesImporter
{
    class Program
    {
        private const string countryPath = "C:\\Source\\Datafiles\\countries.txt";
        private const string regionsPath = "C:\\Source\\Datafiles\\regions.txt";
        private const string citiesPath = "C:\\Source\\Datafiles\\worldcitiespop.txt";
        private const string hotelFilePath = "C:\\Source\\Datafiles\\Hotels_All.csv";

        static void Main(string[] args)
        {
            Countries();
            Regions();
            Cities();
            //HotelsCombined();
            //BuildSearchIndexes();

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey(false);
        }

        static void Countries()
        {
            Console.WriteLine("=== PERPARING COUNTRIES ===");
            var importService = new ImportService();
            importService.PrepareCountries(countryPath);
        }

        static void Regions()
        {
            Console.WriteLine("=== PERPARING REGIONS ===");
            var importService = new ImportService();
            importService.PrepareRegions(regionsPath);
        }

        static void Cities()
        {
            Console.WriteLine("=== PERPARING CITIES ===");
            var importService = new ImportService();
            var numErrors = importService.PrepareCities(citiesPath);
            if (numErrors > 0) Console.Write(String.Format("!!!!!! {0} ERRORS !!!!!!", numErrors));
        }

        static void HotelsCombined()
        {
            Console.WriteLine("=== IMPORTING HOTELS ===");
            ImportService importService = new ImportService();
            var numErrors = importService.ImportHotelsCombinedHotels(hotelFilePath);
            if (numErrors > 0) Console.Write(String.Format("!!!!!! {0} ERRORS !!!!!!", numErrors));
        }

        private static void BuildSearchIndexes()
        {
            var luceneService = new LuceneService();
            luceneService.IndexDestinations();

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
