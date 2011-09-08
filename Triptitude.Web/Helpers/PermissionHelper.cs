﻿using System;
using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
    public static class PermissionHelper
    {
        [Obsolete("use user.OwnsTrips(...) instead.")]
        public static bool UserOwnsTrips(User user, params Trip[] trips)
        {
            if (user == null) return false;

            var userOwnsAllTrips = trips.All(t => t.User == user);
            return userOwnsAllTrips;
        }
    }
}