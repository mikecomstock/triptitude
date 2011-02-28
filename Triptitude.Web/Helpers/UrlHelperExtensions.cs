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
            return url.RouteUrl("login");
        }
        public static string Logout(this UrlHelper url)
        {
            return url.RouteUrl("logout");
        }

        #endregion

        #region Trips

        public static string CreateTrip(this UrlHelper url)
        {
            return url.Action("create", "trips");
        }

        public static string SetDefaultTrip(this UrlHelper url)
        {
            return url.Action("setdefaulttrip", "users");
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
            return url.Action("addtotrip", "websites");
        }

        #endregion

        #region Itinerary Items

        public static string EditItineraryItem(this UrlHelper url, ItineraryItem itineraryItem)
        {
            return url.Action("edit", "itineraryitems", new { id = itineraryItem.Id });
        }

        public static string SoftDeleteItineraryItem(this UrlHelper url, ItineraryItem itineraryItem)
        {
            return url.Action("delete", "itineraryitems", new { id = itineraryItem.Id });
        }

        public static string Details(this UrlHelper url, ItineraryItem itineraryItem)
        {
            if (itineraryItem.BaseItem != null)
            {
                switch (itineraryItem.BaseItem.ItemType)
                {
                    case "H": return HotelDetails(url, itineraryItem.Hotel);
                }
            }

            return Details(url, itineraryItem.Website);
        }

        #endregion

        #region Websites

        public static string Details(this UrlHelper url, Website website)
        {
            return url.Action("details", "websites", new { id = website.Id, title = website.Title.ToSlug() });
        }

        public static string WebsiteThumb(this UrlHelper url, Website website, Website.ThumbSize thumbSize)
        {
            return ConfigurationManager.AppSettings["StaticFolderUrl"] + "/websitethumbs/" + Website.ThumbFilename(website.Id, thumbSize);
        }

        #endregion

        #region Hotels

        public static string HotelDetails(this UrlHelper url, ExpediaHotel hotel)
        {
            return url.Action("details", "hotels", new { id = hotel.BaseItem.Id, title = hotel.BaseItem.Name.ToSlug() });
        }

        #endregion
    }
}