using System.Configuration;
using System.Web.Helpers;
using Hammock;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Services
{
    public class PlacesService
    {
        //private static string FactualServerKey { get { return ConfigurationManager.AppSettings["FactualServerKey"]; } }
        private static string GoogleAPIKey { get { return ConfigurationManager.AppSettings["GoogleAPIKey"]; } }

        //public IEnumerable<Place> Search(PlaceSearchForm form)
        //{
        //    if (string.IsNullOrWhiteSpace(form.Search))
        //        yield break;

        //    string filters = string.Empty;
        //    filters += string.Format(@"""$search"":[""{0}""],", form.Search);
        //    filters += string.Format(@"""$loc"":{{""$within_dist"":[{0},{1},{2}]}}", form.Latitude, form.Longitude, form.RadiusInKm);
        //    string sort = @"""factual_id"":1";

        //    string searchPath = string.Format("http://api.factual.com/v2/tables/s4OOB4/read?APIKey={0}&filters={{{1}}}&sort={{{2}}}", FactualServerKey, filters, sort);
        //    dynamic json = GetJson(searchPath);

        //    foreach (dynamic d in json.response.data)
        //    {
        //        Place p = Place.FromSearchJson(d);
        //        yield return p;
        //    }
        //}

        public Place FindGoogle(string googleReference)
        {
            string path = string.Format("https://maps.googleapis.com/maps/api/place/details/json?reference={0}&sensor=true&key={1}", googleReference, GoogleAPIKey);
            dynamic json = GetJson(path).result;
            Place place = new Place();
            place.Name = json.Name;
            place.GoogId = json.Id;
            place.GoogReference = json.Reference;
            place.Website = json.URL;
            place.Latitude = json.Geometry.Location.Lat;
            place.Longitude = json.Geometry.Location.Lng;

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