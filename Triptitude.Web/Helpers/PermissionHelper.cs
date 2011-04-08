using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
    public static class PermissionHelper
    {
        public static bool UserOwnsTrips(User user, params Trip[] trips)
        {
            if (user == null) return false;

            var userOwnsAllTrips = trips.All(t => user.Trips.Contains(t));
            return userOwnsAllTrips;
        }
    }
}