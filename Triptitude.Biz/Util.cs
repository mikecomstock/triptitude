using System;
using System.Web;

namespace Triptitude.Biz
{
    public static class Util
    {
        public static bool ServerIsProduction
        {
            get { return HttpContext.Current != null && HttpContext.Current.Request.Url.Host.Contains("triptitude.com"); }
        }

        public static string DateTimeRangeString(int? beginDay, TimeSpan? beginTime, int? endDay, TimeSpan? endTime)
        {
            string dateTimeString = string.Empty;

            if (beginDay == endDay && !beginTime.HasValue && !endTime.HasValue)
            {
                dateTimeString += string.Format("Day {0}", beginDay);
            }
            else
            {
                if (beginTime.HasValue) dateTimeString += string.Format("{0} ", DateTime.Today.Add(beginTime.Value).ToShortTimeString());
                dateTimeString += string.Format("Day {0}", beginDay);

                dateTimeString += " - ";

                if (endTime.HasValue) dateTimeString += string.Format("{0} ", DateTime.Today.Add(endTime.Value).ToShortTimeString());
                dateTimeString += string.Format("Day {0}", endDay);
            }

            return dateTimeString;
        }
    }
}
