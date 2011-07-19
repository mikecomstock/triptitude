using System.Collections.Generic;
using System.Configuration;
using System.Web.Helpers;
using Hammock;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Services
{
    public class PlacesService
    {
        private static string FactualServerKey { get { return ConfigurationManager.AppSettings["FactualServerKey"]; } }

        public IEnumerable<Place> Search(PlaceSearchForm form)
        {
            string filters = string.Format(@"{{""$loc"":{{""$within_dist"":[{0},{1},{2}]}}}}&sort={{""factual_id"":1}}", form.Latitude, form.Longitude, form.RadiusInKm);
            string searchPath = string.Format("http://api.factual.com/v2/tables/s4OOB4/read?APIKey={0}&filters={1}", FactualServerKey, filters);
            dynamic json = GetJson(searchPath);

            foreach (dynamic d in json.response.data)
            {
                Place p = Place.FromSearchJson(d);
                yield return p;
            }
        }

        public Place Find(string placeId)
        {
            string path = string.Format("http://www.factual.com/{0}.json", placeId);
            dynamic json = GetJson(path);
            Place place = Place.FromEntityJson(json);
            return place;
        }

        private static dynamic GetJson(string path)
        {
            Hammock.RestRequest request = new RestRequest { Path = path };
            Hammock.RestClient client = new RestClient();
            RestResponse response = client.Request(request);
            string content = response.Content;
            dynamic json = Json.Decode(content);
            return json;
        }
    }
}