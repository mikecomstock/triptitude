using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Services
{
    public class GeoNamesService
    {
        public IEnumerable<Country> GetCountries()
        {
            WebClient webClient = new WebClient();
            string countryInfoFile = webClient.DownloadString("http://download.geonames.org/export/dump/countryInfo.txt");

            foreach (string line in countryInfoFile.Split('\n').Where(l => l.Trim().Length > 0 && !l.StartsWith("#")))
            {
                var c = line.Split('\t');
                Country country = new Country
                                      {
                                          ISO = c[0],
                                          ISO3 = c[1],
                                          ISONumeric = c[2],
                                          FIPS = c[3],
                                          Name = c[4],
                                          Capital = c[5],
                                          AreaInSqKm = c[6],
                                          Population = c[7],
                                          Continent = c[8],
                                          TLD = c[9],
                                          CurrencyCode = c[10],
                                          CurrencyName = c[11],
                                          Phone = c[12],
                                          PostalCodeFormat = c[13],
                                          PostalCodeRegex = c[14],
                                          Languages = c[15],
                                          GeoNameID = c[16],
                                          Neighbours = c[17],
                                          EquivalentFipsCode = c[18]
                                      };

                if (country.GeoNameID != "0")
                    yield return country;
                else
                    yield break;
            }
        }

        //public IEnumerable<GeoName> GetGeonames()
        public void GetGeonames()
        {
            WebClient webClient = new WebClient();

            // Zip Examples at http://wiki.sharpdevelop.net/SharpZipLib-Zip-Samples.ashx
            Stream downloadData = webClient.OpenRead("http://download.geonames.org/export/dump/CR.zip");
            ZipInputStream zipInputStream = new ZipInputStream(downloadData);
            ZipEntry zipEntry = zipInputStream.GetNextEntry();

            // Ignore the readme.txt file
            while (zipEntry.Name == "readme.txt")
            {
                zipEntry = zipInputStream.GetNextEntry();
            }
            
            // Could just return the zipInputStream instead, and read each line as it comes in.
            // Would that make the download slower though? Not sure.

            byte[] buffer = new byte[4096]; // 4K is optimum
            MemoryStream outputStream = new MemoryStream();
            StreamUtils.Copy(zipInputStream, outputStream, buffer);
            outputStream.Position = 0;
            StreamReader reader = new StreamReader(outputStream);


            //TODO: Take care of geonames object creation, table creation, etc! Time to go to Key West now.


            string line = reader.ReadToEnd();

            Console.WriteLine(line);
        }
    }
}