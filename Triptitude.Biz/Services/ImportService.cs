﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Services
{
    public class ImportService
    {
        public void PrepareCountries(string countryInfoPath, string outPath)
        {
            FileStream inFileStream = new FileStream(countryInfoPath, FileMode.Open);
            FileStream outFileStream = new FileStream(outPath, FileMode.Create);
            StreamReader reader = new StreamReader(inFileStream);
            StreamWriter writer = new StreamWriter(outFileStream);

            using (inFileStream)
            using (reader)
            using (outFileStream)
            using (writer)
            {
                string line;
                string[] l;

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line[0] == '#') continue;
                    l = line.Split('\t');

                    if (l[16] == "0") continue;

                    Console.WriteLine(l[4]);

                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}",
                        l[16], l[0], l[1], l[2], l[3], l[4], l[5], l[7], l[8], l[9], l[10], l[11], l[12], l[13], l[14], l[15], l[17], l[18]);

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

        public void PrepareRegions(string allCountriesPath, string outPath)
        {
            CountriesRepo countriesRepo = new CountriesRepo();
            var countries = countriesRepo.FindAll().ToList();

            FileStream inFileStream = new FileStream(allCountriesPath, FileMode.Open);
            FileStream outFileStream = new FileStream(outPath, FileMode.Create);
            StreamReader reader = new StreamReader(inFileStream);
            StreamWriter writer = new StreamWriter(outFileStream);

            using (inFileStream)
            using (reader)
            using (outFileStream)
            using (writer)
            {
                string line;
                string[] l;
                int i = 0;
                Country country;

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    l = line.Split('\t');

                    if (l[6] == "A" && l[7] == "ADM1")
                    {
                        if (++i % 100 == 0) Console.WriteLine(i);
                        country = countries.First(c => c.ISO == l[8]);
                        writer.WriteLine("{0}\t{1}\t{2}\t{3}", l[0], l[2], l[8], country.GeoNameID);
                        // GeoNameID, Name, Admin1Code, CountryGeoNameID
                    }
                }
            }
        }

        public void PrepareCities(string allCountriesPath, string outPath)
        {
            RegionsRepo regionsRepo = new RegionsRepo();
            var regions = regionsRepo.FindAll().ToList();

            Dictionary<string, Region> d = new Dictionary<string, Region>(regions.Count);
            foreach (var region in regions)
            {
                d.Add(region.Country.ISO + "." + region.GeoNameAdmin1Code, region);
            }

            FileStream inFileStream = new FileStream(allCountriesPath, FileMode.Open);
            FileStream outFileStream = new FileStream(outPath, FileMode.Create);
            StreamReader reader = new StreamReader(inFileStream);
            StreamWriter writer = new StreamWriter(outFileStream);

            using (inFileStream)
            using (reader)
            using (outFileStream)
            using (writer)
            {
                string line, key;
                string[] l;
                int i = 0;
                Region region;

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    l = line.Split('\t');

                    if (l[6] == "P")
                    {
                        if (++i % 10000 == 0) Console.WriteLine(i);
                        key = l[8] + "." + l[10];
                        d.TryGetValue(key, out region);
                        if (region != null)
                            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", l[0], l[2], l[4], l[5], region.GeoNameID);
                        // GeoNameID, ASCIIName, Latitude, Longitude, RegionGeoNameID
                    }
                }
            }
        }

        public void ImportHotels(string hotelAllActivePath, string outPath)
        {
            FileStream inFileStream = new FileStream(hotelAllActivePath, FileMode.Open);
            FileStream outFileStream = new FileStream(outPath, FileMode.Create);
            StreamReader reader = new StreamReader(inFileStream);
            StreamWriter writer = new StreamWriter(outFileStream);

            int i = 0;

            using (inFileStream)
            using (reader)
            using (outFileStream)
            using (writer)
            {
                reader.ReadLine(); //ignore first line

                writer.WriteLine("set nocount on");
                writer.WriteLine("begin tran");
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var l = line.Split('|');

                    var name = l[1].Replace("'", "''");
                    var latitude = l[11];
                    var longitude = l[10];
                    var expediaHotelId = int.Parse(l[0]);
                    var hasContinentalBreakfast = l[30] == "Y" ? 1 : 0;

                    var sql = string.Format("execute insert_eh '{0}',{1},{2},{3},{4}",
                                            name,
                                            latitude,
                                            longitude,
                                            expediaHotelId,
                                            hasContinentalBreakfast
                        );
                    //DbProvider._db.Database.SqlCommand(sql);
                    writer.WriteLine(sql);

                    if (++i % 100 == 0)
                    {
                        Console.Clear();
                        Console.WriteLine(i);
                    }
                }
                writer.WriteLine("commit");
                writer.WriteLine("set nocount off");
            }
        }

        public void ImportHotelImages(string hotelImagesPath, string hotelImagesOutPath)
        {
            FileStream inFileStream = new FileStream(hotelImagesPath, FileMode.Open);
            FileStream outFileStream = new FileStream(hotelImagesOutPath, FileMode.Create);
            StreamReader reader = new StreamReader(inFileStream);
            StreamWriter writer = new StreamWriter(outFileStream);

            int i = 0;

            using (inFileStream)
            using (reader)
            using (outFileStream)
            using (writer)
            {
                reader.ReadLine(); //ignore first line

                writer.WriteLine("set nocount on");
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var l = line.Split('|');

                    var expediaHotelId = l[0];
                    var imageURL = l[3];
                    var thumbURL = l[8];
                    var isDefault = l[9] == "True" ? 1 : 0;
                    var height = l[5];
                    var width = l[6];
                    var sql = string.Format("execute insert_eh_photo {0},'{1}','{2}',{3},{4},{5}",
                            expediaHotelId,
                            imageURL,
                            thumbURL,
                            isDefault,
                            height,
                            width
                        );
                    DbProvider._db.Database.SqlCommand(sql);
                    //writer.WriteLine(sql);

                    if (++i % 1000 == 0)
                    {
                        Console.Clear();
                        Console.WriteLine(i);
                    }
                }
                writer.WriteLine("set nocount off");
            }
        }
    }
}