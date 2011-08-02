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
        public static string SlugAction(this UrlHelper url, string action, string controller, object id, string name)
        {
            return url.Action(action, controller, new { idslug = id + "-" + name.ToSlug() });
        }

        #region General

        public static string Login(this UrlHelper url)
        {
            return url.RouteUrl("login");
        }
        public static string Logout(this UrlHelper url)
        {
            return url.RouteUrl("logout");
        }
        public static string ForgotPass(this UrlHelper url)
        {
            return url.RouteUrl("forgotpass");
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

        public static string Admin(this UrlHelper url, Tag tag)
        {
            return url.Action("tag", "admin", new { tag.Id });
        }

        public static string Admin(this UrlHelper url, Item item)
        {
            return url.Action("item", "admin", new { item.Id });
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

        public static string PackingList(this UrlHelper url, Trip trip)
        {
            return url.SlugAction("packinglist", "trips", trip.Id, trip.Name);
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
            return url.Action("addhotel", "activities");
        }
        public static string ItineraryEditHotel(this UrlHelper url)
        {
            return url.Action("edithotel", "activities");
        }


        public static string ItineraryAddTransportation(this UrlHelper url)
        {
            return url.Action("addtransportation", "activities");
        }
        public static string ItineraryEditTransportation(this UrlHelper url)
        {
            return url.Action("edittransportation", "activities");
        }


        public static string ItineraryAddWebsite(this UrlHelper url)
        {
            return url.Action("addwebsite", "activities");
        }
        public static string ItineraryEditWebsite(this UrlHelper url)
        {
            return url.Action("editwebsite", "activities");
        }
        

        public static string ItineraryAddPlace(this UrlHelper url)
        {
            return url.Action("addplace", "activities");
        }
        public static string ItineraryEditPlace(this UrlHelper url)
        {
            return url.Action("editplace", "activities");
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

        public static string Places(this UrlHelper url, IDestination destination)
        {
            return SlugAction(url, "places", "destinations", destination.GeoNameID, destination.FullName);
        }

        #endregion

        #region Tags

        public static string Details(this UrlHelper url, Tag tag)
        {
            return SlugAction(url, "details", "tags", tag.Id, tag.Name);
        }

        public static string Tag(this UrlHelper url, IDestination destination, Tag tag)
        {
            return url.Action("activity", "destinations", new { idslug = destination.GeoNameID + "-" + destination.FullName.ToSlug(), tagidslug = tag.Id + "-" + tag.Name.ToSlug() });
        }

        #endregion

        #region Places

        public static string Details(this UrlHelper url, Place place)
        {
            return url.Action("details", "places", new { id = place.FactualId });
        }

        #endregion
    }
}