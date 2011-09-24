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

        public static string Admin(this UrlHelper url, string controller, string actionName, object id = null)
        {
            return url.Action(actionName, "admin" + controller, new { id }, null);
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

        public static string CreateTrip(this UrlHelper url, Place to = null)
        {
            return url.Action("create", "trips", new { to = to == null ? string.Empty : to.Id.ToString() });
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

        public static string Delete(this UrlHelper url, int activityId)
        {
            return url.Action("delete", "activities", new { id = activityId });
        }

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


        public static string ItineraryAddPlace(this UrlHelper url)
        {
            return url.Action("addplace", "activities");
        }
        public static string ItineraryEditPlace(this UrlHelper url)
        {
            return url.Action("editplace", "activities");
        }

        #endregion

        //#region Notes

        //public static string CreateNote(this UrlHelper url)
        //{
        //    return url.Action("create", "notes");
        //}

        //#endregion

        #region Hotels

        public static string Details(this UrlHelper url, Hotel hotel)
        {
            return SlugAction(url, "details", "hotels", hotel.Id, hotel.Name);
        }

        #endregion

        #region Tags

        public static string Details(this UrlHelper url, Tag tag)
        {
            return SlugAction(url, "details", "tags", tag.Id, tag.Name);
        }

        #endregion

        #region Places

        public static string Details(this UrlHelper url, Place place)
        {
            return SlugAction(url, "details", "places", place.Id, place.Name);
        }
        public static string Nearby(this UrlHelper url, Place place)
        {
            return SlugAction(url, "nearby", "places", place.Id, place.Name);
        }
        public static string PlaceSearch(this UrlHelper url)
        {
            return url.Action("search", "places");
        }

        #endregion
    }
}