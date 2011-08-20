using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class CitiesRepo : Repo<City>
    {
        public IEnumerable<City> Search(CitySearchForm form)
        {
            int radiusInMeters = (int)(form.RadiusInMiles * 1609.344);

            string sql = "select c.* from Cities c join CitiesNear(@p0,@p1,@p2) cn on c.GeoNameId = cn.City_GeoNameId";
            var cities = Sql(sql, form.Latitude, form.Longitude, radiusInMeters);
            return cities;
        }

        public IEnumerable<City> GetDataReaderForIndexing()
        {
            var regions = new RegionsRepo().FindAll().ToDictionary(r => r.GeoNameID);

            DbConnection dbConnection = _db.Database.Connection;
            DbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "select * from cities";

            if (dbConnection.State != ConnectionState.Open)
                dbConnection.Open();
            var dbDataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);

            while (dbDataReader.Read())
            {
                yield return new City
                                 {
                                     GeoNameID = (int)dbDataReader["GeonameId"],
                                     ASCIIName = (string)dbDataReader["ASCIIName"],
                                     Region = regions[(int)dbDataReader["Region_GeoNameId"]]
                                 };
            }
        }
    }
}