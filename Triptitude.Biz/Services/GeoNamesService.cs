using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Services
{
    public class GeoNamesService
    {
        public IEnumerable<Country> GetCountries()
        {
            FileStream fs = new FileStream("C:\\Users\\Mike\\Desktop\\Temp\\countryInfo.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fs);

            using (fs)
            using (reader)
            {
                string line;
                string[] c;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line[0] == '#') continue;
                    c = line.Split('\t');

                    Country country = new Country
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
                                          };

                    if (country.GeoNameID != 0)
                        yield return country;
                }
            }
        }

        public IEnumerable<Region> GetAdmin1Regions()
        {
            CountriesRepo countriesRepo = new CountriesRepo();
            var countries = countriesRepo.FindAll().ToList();

            FileStream fs = new FileStream("C:\\Users\\Mike\\Desktop\\Temp\\allCountries\\allCountries.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fs);

            using (fs)
            using (reader)
            {
                string line;
                string[] l;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    l = line.Split('\t');

                    if (l[6] == "A" && l[7] == "ADM1")
                    {
                        Region region = new Region
                        {
                            GeoNameID = int.Parse(l[0]),
                            ASCIIName = l[2],
                            Country = countries.First(c => c.ISO == l[8]),
                            GeoNameAdmin1Code = l[10]
                        };

                        yield return region;
                    }
                }
            }
        }

        public void GetCities()
        {
            RegionsRepo regionsRepo = new RegionsRepo();
            var regions = regionsRepo.FindAll().ToList();

            Dictionary<string, Region> d = new Dictionary<string, Region>(regions.Count);
            foreach (var region in regions)
            {
                d.Add(region.Country.ISO + "." + region.GeoNameAdmin1Code, region);
            }

            FileStream fs = new FileStream("C:\\Users\\Mike\\Desktop\\Temp\\allCountries\\allCountries.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fs);

            FileStream fsOut = new FileStream("C:\\Users\\Mike\\Desktop\\Temp\\allCountries\\allCountries-Cities.txt", FileMode.Create);
            StreamWriter write = new StreamWriter(fsOut);

            int i = 0;

            using (fs)
            using (reader)
            using (fsOut)
            using (write)
            {
                string line, key;
                string[] l;
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
                            write.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", l[0], l[2], l[4], l[5], region.GeoNameID);
                    }
                }
            }
        }
    }
}