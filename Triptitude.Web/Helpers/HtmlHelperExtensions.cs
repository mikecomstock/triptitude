using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString SelectTripDropDownList(this HtmlHelper html, IEnumerable<Trip> trips, Trip selectedTrip)
        {
            return html.DropDownList("id", trips.ToList().Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString(), Selected = t.Id == selectedTrip.Id }));
        }

        public static MvcHtmlString SelectTransportationTypeDropDownList(this HtmlHelper html, IEnumerable<TransportationType> transportationTypes, int selectedTypeId)
        {
            var options = transportationTypes.Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString(), Selected = t.Id == selectedTypeId });
            return html.DropDownList("transportationtypeid", options);
        }
    }
}