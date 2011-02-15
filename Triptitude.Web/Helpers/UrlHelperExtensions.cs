using System.Configuration;
using System.Web.Mvc;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        #region General

        public static string Login(this UrlHelper url)
        {
            return url.RouteUrl("Login");
        }
        public static string Logout(this UrlHelper url)
        {
            return url.RouteUrl("Logout");
        }

        #endregion

        #region Trips

        public static string CreateTrip(this UrlHelper url)
        {
            return url.Action("Create", "Trips");
        }

        public static string SetDefaultTrip(this UrlHelper url)
        {
            return url.Action("SetDefaultTrip", "Users");
        }

        public static string TripDetails(this UrlHelper url, Trip trip)
        {
            return url.Action("details", "trips", new { id = trip.Id, title = trip.Name.ToSlug() });
        }

        public static string TripSettings(this UrlHelper url, Trip trip)
        {
            return url.Action("settings", "trips", new { id = trip.Id, title = trip.Name.ToSlug() });
        }

        public static string AddWebsiteToTrip(this UrlHelper url)
        {
            return url.Action("AddToTrip", "Website");
        }

        #endregion

        #region Itinerary Items

        public static string EditItineraryItem(this UrlHelper url, ItineraryItem itineraryItem)
        {
            return url.Action("edit", "itineraryitems", new { id = itineraryItem.Id });
        }

        #endregion

        public static string WebsiteThumb(this UrlHelper url, Website website, Website.ThumbSize thumbSize)
        {
            return ConfigurationManager.AppSettings["StaticFolderUrl"] + "/WebsiteThumbs/" + Website.ThumbFilename(website.Id, thumbSize);
        }
    }
}