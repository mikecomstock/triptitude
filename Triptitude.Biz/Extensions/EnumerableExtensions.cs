using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || !e.Any();
        }

        #region PackingListItem
        public static IEnumerable<PackingListItem> Public(this IEnumerable<PackingListItem> items)
        {
            return items.Where(e => e.Public);
        }
        public static IOrderedEnumerable<IGrouping<string, PackingListItem>> GroupedAndOrderedByCountDesc(this IEnumerable<PackingListItem> items)
        {
            var groupBy = items.GroupBy(i => i.Name);
            var orderBy = groupBy.OrderByDescending(g => g.Count()).ThenBy(g => g.Key);
            return orderBy;
        }
        #endregion
    }
}