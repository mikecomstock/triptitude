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

        public static string Background(this UrlHelper url, string imageFileName)
        {
            return Path.Combine("/static/backgrounds/", imageFileName);
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
            return Path.Combine("/static/", path);
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

        public static string TripsHome(this UrlHelper url)
        {
            return url.Action("index", "trips");
        }

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

        public static string Details(this UrlHelper url, ItineraryItem itineraryItem)
        {
            if (itineraryItem.Hotel != null)
                return Details(url, itineraryItem.Hotel);

            if (itineraryItem.Website != null)
                return Details(url, itineraryItem.Website);

            throw new Exception("Unknown itineraryitem type. Could not find details url.");
        }

        public static string ItineraryAddHotel(this UrlHelper url, Hotel hotel)
        {
            return url.Action("addhotel", "itineraryitems", new { hotelId = hotel.Id });
        }
        public static string ItineraryEditHotel(this UrlHelper url, ItineraryItem itineraryItem)
        {
            return url.Action("edithotel", "itineraryitems", new { itineraryItemId = itineraryItem.Id });
        }

        public static string ItineraryAddTransportation(this UrlHelper url)
        {
            return url.Action("addtransportation", "itineraryitems");
        }

        public static string ItineraryEditTransportation(this UrlHelper url, Transportation transportation)
        {
            return url.Action("edittransportation", "itineraryitems", new { transportation.Id });
        }

        #endregion

        #region Notes

        public static string CreateNote(this UrlHelper url, int itineraryItemId)
        {
            return url.Action("create", "notes", new { itineraryItemId });
        }

        public static string Edit(this UrlHelper url, Note note)
        {
            return url.Action("edit", "notes", new { note.Id });
        }

        public static string Delete(this UrlHelper url, Note note)
        {
            return url.Action("delete", "notes", new { note.Id });
        }

        #endregion

        #region Websites

        public static string CreateWebsite(this UrlHelper url)
        {
            return url.Action("create", "websites");
        }

        public static string EditWebsite(this UrlHelper url)
        {
            return url.Action("edit", "websites");
        }

        public static string Details(this UrlHelper url, Website website)
        {
            return url.SlugAction("details", "websites", website.Id, website.Title);
        }

        public static string WebsiteThumb(this UrlHelper url, Website website, Website.ThumbSize thumbSize)
        {
            return ConfigurationManager.AppSettings["StaticFolderUrl"] + "/websitethumbs/" + Website.ThumbFilename(website.Id, thumbSize);
        }

        #endregion

        #region Hotels

        public static string HotelsHome(this UrlHelper url)
        {
            return url.Action("index", "hotels");
        }

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

        public static string Details(this UrlHelper url, Destination destination)
        {
            return SlugAction(url, "details", "destinations", destination.Id, destination.FullName);
        }

        public static string Hotels(this UrlHelper url, Destination destination)
        {
            return SlugAction(url, "hotels", "destinations", destination.Id, destination.FullName);
        }

        #endregion
    }
}