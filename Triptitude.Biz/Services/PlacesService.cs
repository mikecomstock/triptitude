using System.Configuration;
using System.Web.Helpers;
using Hammock;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Services
{
    public class PlacesService
    {
        private static string GoogleAPIKey { get { return ConfigurationManager.AppSettings["GoogleAPIKey"]; } }

        public Place CreateFromGoogle(string googleReference)
        {
            string path = string.Format("https://maps.googleapis.com/maps/api/place/details/json?reference={0}&sensor=true&key={1}", googleReference, GoogleAPIKey);
            dynamic json = GetJson(path).result;
            Place place = new Place
                              {
                                  Name = json.Name,
                                  GoogId = json.Id,
                                  GoogReference = json.Reference,
                                  Website = json.URL,
                                  Latitude = json.Geometry.Location.Lat,
                                  Longitude = json.Geometry.Location.Lng,
                                  AddressExtended = json.formatted_address,
                                  Telephone = json.formatted_phone_number
                              };

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