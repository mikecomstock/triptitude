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

        public static MvcHtmlString AddToTripButton(this HtmlHelper html, UrlHelper url, BaseItem baseItem)
        {
            string s = string.Format("<a href=\"{0}\" class=\"add-to-trip-button\">Add to Trip</a>", url.AddBaseItemToItinerary(baseItem));
            return new MvcHtmlString(s);
        }
    }
}