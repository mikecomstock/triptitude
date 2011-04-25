using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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

            using (FileStream inFileStream = new FileStream(countryInfoPath, FileMode.Open))
            using (StreamReader reader = new StreamReader(inFileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line[0] == '#') continue;
                    var l = line.Split('\t');

                    if (l[16] == "0") continue;

                    var geonameId = l[16];
                    var iso = l[0];
                    var name = l[4];

                    Console.WriteLine(name);

                    repo.ExecuteSql("insert into Countries (GeoNameId, ISO, Name) values (@p0, @p1, @p2)", geonameId, iso, name);
                }
            }

            /*Country country = new Country
                {
                    ISO = c[0],
                    ISO3 = c[1],
                    ISONumeric = int.Parse(c[2]),
                    FIPS = c[3],
                    Name = c[4],
                    Capital = c[5],
                    AreaInSqKm = string.IsNullOrWhiteSpace(c[6]) ? (double?)null : double.Parse(c[6]),
                    Population = int.Parse(c[7]),
                    Continent = c[8],
                    TLD = c[9],
                    CurrencyCode = c[10],
                    CurrencyName = c[11],
                    Phone = c[12],
                    PostalCodeFormat = c[13],
                    PostalCodeRegex = c[14],
                    Languages = c[15],
                    GeoNameID = int.Parse(c[16]),
                    Neighbours = c[17],
                    EquivalentFipsCode = c[18]
                };*/
        }

        public void PrepareRegions(string allCountriesPath)
        {
            repo.ExecuteSql("truncate table regions");

            CountriesRepo countriesRepo = new CountriesRepo();
            var countries = countriesRepo.FindAll().ToDictionary(c => c.ISO, c => c.Id);

            using (FileStream inFileStream = new FileStream(allCountriesPath, FileMode.Open))
            using (StreamReader reader = new StreamReader(inFileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var l = line.Split('\t');

                    if (l[6] == "A" && l[7] == "ADM1")
                    {
                        var geonameId = l[0];
                        var name = l[2];
                        var admin1Code = l[10];
                        var countryId = countries[l[8]];

                        Console.WriteLine(name);

                        repo.ExecuteSql("insert into Regions (GeoNameId, ASCIIName, GeoNameAdmin1Code, Country_Id) values (@p0, @p1, @p2, @p3)", geonameId, name, admin1Code, countryId);
                    }
                }
            }
        }

        public int PrepareCities(string allCountriesPath)
        {
            repo.ExecuteSql("truncate table cities");

            CountriesRepo countriesRepo = new CountriesRepo();
            var countries = countriesRepo.FindAll().ToDictionary(d => d.ISO, d => d.Regions.ToDictionary(r => r.GeoNameAdmin1Code, r => r.Id));

            int numErrors = 0;

            using (FileStream inFileStream = new FileStream(allCountriesPath, FileMode.Open))
            using (StreamReader reader = new StreamReader(inFileStream))
            {
                int i = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var l = line.Split('\t');

                    int population;
                    int.TryParse(l[14], out population);

                    if (l[6] == "P")
                    {
                        var geoNameId = l[0];
                        var ASCIIName = l[2];
                        var lat = l[4];
                        var lon = l[5];
                        var countryISO = l[8];
                        var regionISO = l[10];

                        if (++i % 100 == 0) Console.WriteLine(i);
                        if (i == 10000) return numErrors;

                        if (countries.ContainsKey(countryISO) && countries[countryISO].ContainsKey(regionISO))
                        {
                            var regionId = countries[countryISO][regionISO];
                            repo.ExecuteSql("execute InsertCity @p0, @p1, @p2, @p3, @p4", geoNameId, ASCIIName, lat, lon, regionId);
                        }
                        else
                        {
                            numErrors++;
                        }
                    }
                }
            }

            return numErrors;
        }

        /// <returns>Number of errors</returns>
        public int ImportHotelsCombinedHotels(string hotelFilePath)
        {
            repo.ExecuteSql("truncate table hotels");

            int i = 0, numErrors = 0;

            using (FileStream inFileStream = new FileStream(hotelFilePath, FileMode.Open))
            using (StreamReader reader = new StreamReader(inFileStream))
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
                        Console.WriteLine(i);
                }
            }

            return numErrors;
        }

        //public void ImportHotelImages(string hotelImagesPath, string hotelImagesOutPath)
        //{
        //    FileStream inFileStream = new FileStream(hotelImagesPath, FileMode.Open);
        //    FileStream outFileStream = new FileStream(hotelImagesOutPath, FileMode.Create);
        //    StreamReader reader = new StreamReader(inFileStream);
        //    StreamWriter writer = new StreamWriter(outFileStream);

        //    int i = 0;
        //    Repo repo = new Repo();

        //    using (inFileStream)
        //    using (reader)
        //    using (outFileStream)
        //    using (writer)
        //    {
        //        reader.ReadLine(); //ignore first line

        //        writer.WriteLine("set nocount on");
        //        while (!reader.EndOfStream)
        //        {
        //            var line = reader.ReadLine();
        //            var l = line.Split('|');

        //            var hotelId = l[0];
        //            var imageURL = l[3];
        //            var thumbURL = l[8];
        //            var isDefault = l[9] == "True" ? 1 : 0;
        //            var height = l[6];
        //            var width = l[5];

        //            const string sql = "execute InsertHotelPhoto @p0, @p1, @p2, @p3, @p4, @p5";
        //            repo.ExecuteSql(sql, hotelId, imageURL, thumbURL, isDefault, height, width);
        //            //writer.WriteLine(sql);

        //            if (++i % 1000 == 0)
        //            {
        //                Console.Clear();
        //                Console.WriteLine(i);
        //            }
        //        }
        //        writer.WriteLine("set nocount off");
        //    }
        //}
    }
}