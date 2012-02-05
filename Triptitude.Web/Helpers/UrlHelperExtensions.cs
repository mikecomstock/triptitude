using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        private readonly static string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static string SlugAction(this UrlHelper url, string action, string controller, object id, string name, string area = null)
        {
            return url.Action(action, controller, new { idslug = id + "-" + name.ToSlug(), area });
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
            string staticRoot = ConfigurationManager.AppSettings["StaticRoot"];
            var result = Path.Combine(staticRoot, string.Format("v{0}/static/", version), path);
            return result;
        }

        #endregion

        #region Admin

        public static string Admin(this UrlHelper url, string controller = "home", string actionName = "index", object routeValues = null)
        {
            return url.Action(actionName, "admin" + controller, routeValues, null);
        }

        #endregion

        #region Blogs

        public static string BlogsCategory(this UrlHelper url, string category)
        {
            return url.RouteUrl("blogs_category", new { category });
        }

        public static string BlogPost(this UrlHelper url, BlogPost post)
        {
            return url.SlugAction("details", "blogs", post.Id, post.Title, "blogs");
        }

        #endregion

        #region Users

        public static string Details(this UrlHelper url, User user)
        {
            return url.SlugAction("details", "users", user.Id, user.FirstNameLastInitial);
        }

        #endregion

        #region 'My' pages

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

        #region Itinerary

        public static string Delete(this UrlHelper url, int activityId)
        {
            return url.Action("delete", "activities", new { id = activityId });
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

        #region Packing

        public static string PackingListIndex(this UrlHelper url)
        {
            return url.Action("index", "packing");
        }

        public static string SavePackingItem(this UrlHelper url)
        {
            return url.Action("save", "packing");
        }

        public static string Delete(this UrlHelper url, PackingListItem pli)
        {
            return url.Action("delete", "packing", new { id = pli.Id });
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