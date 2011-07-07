using System.Collections.Generic;
using System.Configuration;
using System.Web.Helpers;
using Hammock;

namespace Triptitude.Biz.Services
{
    public class PlacesService
    {
        private string FactualServerKey { get { return ConfigurationManager.AppSettings["FactualServerKey"]; } }

        public IEnumerable<Place> Search(string s)
        {
            string filters = @"{""$loc"":{""$within_dist"":[42.3583333,-71.0602778,5000]}}&sort={""factual_id"":1}";
            string searchPath = string.Format("http://api.factual.com/v2/tables/s4OOB4/read?APIKey={0}&filters={1}", FactualServerKey, filters);

            Hammock.RestRequest request = new RestRequest { Path = searchPath };

            Hammock.RestClient client = new RestClient();
            RestResponse response = client.Request(request);
            string content = response.Content;

            dynamic json = Json.Decode(content);

            foreach (dynamic d in json.response.data)
            {
                //"subject_key","factual_id","name","address","address_extended","po_box","locality","region","country","postcode","tel","fax","category","website","email","latitude","longitude","status"
                Place p = new Place
                              {
                                  FactualId = d[1],
                                  Name = d[2],
                                  Address = d[3],
                                  AddressExtended = d[4],
                                  POBox = d[5],
                                  Locality = d[6],
                                  Region = d[7],
                                  Country = d[8],
                                  PostCode = d[9],
                                  Telephone = d[10],
                                  Fax = d[11],
                                  Category = d[12],
                                  Website = d[13],
                                  Email = d[14],
                                  Latitude = d[15],
                                  Longitude = d[16],
                                  Status = d[17]
                              };
                yield return p;
            }
        }
    }

    public class Place
    {
        public string FactualId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AddressExtended { get; set; }
        public string POBox { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Category { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Status { get; set; }
    }
}