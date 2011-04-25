using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Services
{
    public class ImportService
    {
        private Repo repo = new Repo();

        public void PrepareCountries(string countryInfoPath)
        {
            repo.ExecuteSql("truncate table countries");

            FileStream inFileStream = new FileStream(countryInfoPath, FileMode.Open);
            StreamReader reader = new StreamReader(inFileStream);

            using (inFileStream)
            using (reader)
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var l = line.Split(',');

                    var iso = l[0];
                    var name = l[1].Replace("\"", " ").Trim();

                    if (iso == "O1" || iso == "A1" || iso == "A2") continue;

                    Console.WriteLine(iso + "\t" + name);
                    repo.ExecuteSql("insert into Countries (ISO, Name) values (@p0, @p1)", iso, name);
                }
            }
        }

        public void PrepareRegions(string allCountriesPath)
        {
            repo.ExecuteSql("truncate table regions");

            CountriesRepo countriesRepo = new CountriesRepo();
            var countries = countriesRepo.FindAll().ToList();

            FileStream inFileStream = new FileStream(allCountriesPath, FileMode.Open);
            StreamReader reader = new StreamReader(inFileStream);
            int n = 0;
            using (inFileStream)
            using (reader)
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var l = line.Split(',');

                    var countryISO = l[0];
                    var ISOFIPS = l[1];
                    var name = l[2].Replace("\"", " ").Trim();

                    Country country = countries.First(c => c.ISO == countryISO);

                    // Ignore US FIPS codes (they are dupes of the ISO codes)
                    if (country.ISO == "US" && int.TryParse(ISOFIPS[0].ToString(), out n)) continue;

                    repo.ExecuteSql("insert into Regions (ISOFIPS, Name, Country_Id) values (@p0, @p1, @p2)", ISOFIPS, name, country.Id);

                    Console.WriteLine(string.Format("{0}\t{1}\t{2}", ISOFIPS, name, country.Name));
                }
            }

            // Maxmind data isn't the best so...
            foreach (var country in countries)
            {
                repo.ExecuteSql("insert into Regions (ISOFIPS, Name, Country_Id) values (@p0, @p1, @p2)", "--", "Unknown Region", country.Id);
            }
        }

        public int PrepareCities(string allCountriesPath)
        {
            repo.ExecuteSql("truncate table cities");

            CountriesRepo countriesRepo = new CountriesRepo();
            var countries = countriesRepo.FindAll().ToDictionary(d => d.ISO, d => d.Regions.ToDictionary(r => r.ISOFIPS, r => r.Id));

            int i = 0, numErrors = 0;

            using (FileStream inFileStream = new FileStream(allCountriesPath, FileMode.Open))
            using (StreamReader reader = new StreamReader(inFileStream, Encoding.GetEncoding(28591))) // uses latin-1 encoding (see http://stackoverflow.com/questions/370801/streamreader-problem-unknown-file-encoding-western-iso-88591)
            {
                // Skip first line
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var l = line.Split(',');

                    var countryISO = l[0].ToUpper();
                    var ASCIIName = l[1];
                    var accentName = l[2];
                    var regionISOFIPS = l[3];
                    var population = l[4];
                    var lat = l[5];
                    var lon = l[6];

                    if (countries.ContainsKey(countryISO) && countries[countryISO].ContainsKey(regionISOFIPS))
                    {
                        int regionId = countries[countryISO][regionISOFIPS];

                        if (i % 100 == 0)
                            repo.ExecuteSql("execute InsertCity @p0, @p1, @p2, @p3, @p4, @p5", ASCIIName, accentName, lat, lon, regionId, population);
                    }
                    else
                    {
                        int regionId = countries[countryISO]["--"];

                        if (i % 100 == 0)
                            repo.ExecuteSql("execute InsertCity @p0, @p1, @p2, @p3, @p4, @p5", ASCIIName, accentName, lat, lon, regionId, population);

                        numErrors++;
                    }

                    if (++i % 1000 == 0)
                        Console.WriteLine(i + " " + accentName);
                }
            }

            return numErrors;
        }

        /// <returns>Number of errors</returns>
        public int ImportHotelsCombinedHotels(string hotelFilePath)
        {
            FileStream inFileStream = new FileStream(hotelFilePath, FileMode.Open);
            StreamReader reader = new StreamReader(inFileStream);

            int i = 0, numErrors = 0;

            using (inFileStream)
            using (reader)
            {
                reader.ReadLine(); //ignore first line

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var l = line.Split(',');

                    var hotelsCombinedId = int.Parse(l[0]);
                    var name = l[2].Replace("\"", " ").Trim();
                    var latitude = l[17];
                    var longitude = l[18];
                    var imageId = l[13];
                    var numberOfReviews = l[19];
                    var consumerRating = l[20];

                    if (name != string.Empty && latitude != string.Empty && longitude != string.Empty && imageId != string.Empty)
                    {
                        const string sql = "execute InsertHotel @p0, @p1, @p2, @p3, @p4, @p5, @p6";
                        repo.ExecuteSql(sql, hotelsCombinedId, name, latitude, longitude, imageId, numberOfReviews, consumerRating);
                    }
                    else
                    {
                        numErrors++;
                    }

                    if (++i % 100 == 0)
                    {
                        Console.Clear();
                        Console.WriteLine(i);
                    }
                }
            }

            return numErrors;
        }
    }
}