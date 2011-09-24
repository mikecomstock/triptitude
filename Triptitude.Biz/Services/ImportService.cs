using System;
using System.IO;

namespace Triptitude.Biz.Services
{
    public class ImportService
    {
        private Repo repo = new Repo();
        
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
                        try
                        {
                            repo.ExecuteSql(sql, hotelsCombinedId, name, latitude, longitude, imageId, numberOfReviews,
                                            consumerRating);
                        }
                        catch
                        {
                            numErrors++;
                        }
                    }
                    else
                    {
                        numErrors++;
                    }

                    if (++i % 100 == 0) Console.WriteLine(i);
                }
            }

            return numErrors;
        }
    }
}