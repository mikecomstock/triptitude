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
            return html.DropDownList("trip",
                              trips.ToList().Select(
                                  t =>
                                  new SelectListItem() { Text = t.Name, Value = t.Id.ToString(), Selected = t.Id == selectedTrip.Id }));
        }
    }
    public static class UrlHelperExtensions
    {
        public static string Login(this UrlHelper url)
        {
            return url.RouteUrl("Login");
        }
        public static string Logout(this UrlHelper url)
        {
            return url.RouteUrl("Logout");
        }

        public static string CreateTrip(this UrlHelper url)
        {
            return url.Action("Create", "Trips");
        }

        public static string TripDetails(this UrlHelper url, Trip trip)
        {
            return url.Action("Details", "Trips", new { id = trip.Id });
        }
    }
}