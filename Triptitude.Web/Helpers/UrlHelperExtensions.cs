using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string SlugAction(this UrlHelper url, string action, string controller, int id, string name)
        {
            return url.Action(action, controller, new { idslug = id + "-" + name.ToSlug() });
        }

        #region General

        public static string Signup(this UrlHelper url)
        {
            return url.RouteUrl("signup");
        }
        public static string Login(this UrlHelper url)
        {
            return url.RouteUrl("login");
        }
        public static string Logout(this UrlHelper url)
        {
            return url.RouteUrl("logout");
        }

        public static string Static(this UrlHelper url, string path)
        {
            if (path[0] == '/') throw new ArgumentException("Argument should not begin with a slash.", "path");
            return Path.Combine("/static/", path);
        }

        #endregion

        #region Admin

        public static string Admin(this UrlHelper url, Trip trip)
        {
            return url.Action("trips", "admin", new { trip.Id });
        }

        #endregion

        #region User pages

        public static string MyAccount(this UrlHelper url)
        {
            return url.Action("account", "my");
        }

        public static string MyTrips(this UrlHelper url)
        {
            return url.Action("trips", "my");
        }

        public static string MySettings(this UrlHelper url)
        {
            return url.Action("settings", "my");
        }

        #endregion

        #region Trips

        public static string Details(this UrlHelper url, Trip trip)
        {
            return url.SlugAction("details", "trips", trip.Id, trip.Name);
        }

        #endregion

        #region Plan Trip

        public static string CreateTrip(this UrlHelper url)
        {
            return url.Action("create", "trips");
        }

        public static string SetDefaultTrip(this UrlHelper url, Trip trip)
        {
            return url.Action("defaulttrip", "my", new { id = trip.Id });
        }

        public static string Settings(this UrlHelper url, Trip trip)
        {
            return url.SlugAction("settings", "trips", trip.Id, trip.Name);
        }

        public static string Print(this UrlHelper url, Trip trip)
        {
            return url.SlugAction("print", "trips", trip.Id, trip.Name);
        }

        public static string Map(this UrlHelper url, Trip trip)
        {
            return url.SlugAction("map", "trips", trip.Id, trip.Name);
        }

        #endregion

        #region Itinerary Items

        public static string ItineraryAddHotel(this UrlHelper url)
        {
            return url.Action("addhotel", "itineraryitems");
        }
        public static string ItineraryEditHotel(this UrlHelper url)
        {
            return url.Action("edithotel", "itineraryitems");
        }

        public static string ItineraryAddTransportation(this UrlHelper url)
        {
            return url.Action("addtransportation", "itineraryitems");
        }

        public static string ItineraryEditTransportation(this UrlHelper url)
        {
            return url.Action("edittransportation", "itineraryitems");
        }

        public static string ItineraryAddWebsite(this UrlHelper url)
        {
            return url.Action("addwebsite", "itineraryitems");
        }

        public static string ItineraryEditWebsite(this UrlHelper url)
        {
            return url.Action("editwebsite", "itineraryitems");
        }

        public static string ItineraryAddDestinationTag(this UrlHelper url)
        {
            return url.Action("adddestinationtag", "itineraryitems");
        }

        public static string ItineraryEditDestinationTag(this UrlHelper url)
        {
            return url.Action("editdestinationtag", "itineraryitems");
        }

        #endregion

        #region Websites

        public static string WebsiteThumb(this UrlHelper url, WebsiteActivity websiteActivity, WebsiteActivity.ThumbSize thumbSize)
        {
            return ConfigurationManager.AppSettings["StaticFolderUrl"] + "/websitethumbs/" + WebsiteActivity.ThumbFilename(websiteActivity.Id, thumbSize);
        }

        #endregion

        #region Hotels

        public static string Details(this UrlHelper url, Hotel hotel)
        {
            return SlugAction(url, "details", "hotels", hotel.Id, hotel.Name);
        }

        #endregion

        #region Destinations

        public static string DestinationSearch(this UrlHelper url)
        {
            return url.Action("search", "destinations");
        }

        public static string Details(this UrlHelper url, IDestination destination)
        {
            return SlugAction(url, "details", "destinations", destination.GeoNameID, destination.FullName);
        }

        public static string Hotels(this UrlHelper url, IDestination destination)
        {
            return SlugAction(url, "hotels", "destinations", destination.GeoNameID, destination.FullName);
        }

        public static string Activities(this UrlHelper url, IDestination destination)
        {
            return SlugAction(url, "activities", "destinations", destination.GeoNameID, destination.FullName);
        }

        #endregion
    }
}