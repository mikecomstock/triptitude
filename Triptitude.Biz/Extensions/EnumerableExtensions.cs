using System;
using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Extensions
{
    public static class EnumExtensions
    {
        public static string GetClassName(this Visibility visibility)
        {
            return "visibility-" + visibility.ToString().ToLower();
        }
    }

    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || !e.Any();
        }

        public static IEnumerable<PackingListItem> Public(this IEnumerable<PackingListItem> items)
        {
            return items.Where(e => e.Visibility_Id == 0);
        }

        //TODO: Is this used anywhere?
        public static IEnumerable<Trip> Public(this IEnumerable<Trip> trips)
        {
            //TODO: make setting for public/private trips
            return trips.Where(t => true);
        }
    }

    public static class DateExtensions
    {
        public static bool HasTimePart(this DateTime d)
        {
            return d.TimeOfDay != new TimeSpan(0, 0, 0, 0);
        }
    }
}