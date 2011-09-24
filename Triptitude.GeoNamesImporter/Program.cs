using System;
using Triptitude.Biz.Services;

namespace Triptitude.GeoNamesImporter
{
    class Program
    {
        private const string hotelFilePath = "C:\\Source\\Datafiles\\Hotels_All.csv";

        static DateTime startTime = DateTime.Now;

        static void Main(string[] args)
        {
            //HotelsCombined();

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey(false);
        }

        static void WriteTime()
        {
            Console.WriteLine((DateTime.Now - startTime).TotalMinutes + " minutes");
        }

        static void HotelsCombined()
        {
            Console.WriteLine("=== IMPORTING HOTELS ===");
            ImportService importService = new ImportService();
            var numErrors = importService.ImportHotelsCombinedHotels(hotelFilePath);
            if (numErrors > 0) Console.Write(String.Format("!!!!!! {0} ERRORS !!!!!!", numErrors));
            WriteTime();
        }

    }
}
