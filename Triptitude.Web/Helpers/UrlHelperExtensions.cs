﻿using System.Configuration;
using System.Web.Mvc;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
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

        public static string AddWebsiteToTrip(this UrlHelper url)
        {
            return url.Action("AddToTrip", "Website");
        }

        public static string WebsiteThumb(this UrlHelper url, Website website, Website.ThumbSize thumbSize)
        {
            return ConfigurationManager.AppSettings["StaticFolderUrl"] + "/WebsiteThumbs/" + Website.ThumbFilename(website.Id, thumbSize);
        }
    }
}