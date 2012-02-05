using System;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class HistoriesRepo : Repo<History>
    {
        public History Create(User user, Trip trip, HistoryAction action, HistoryTable table, int tableId)
        {
            History h = new History
                            {
                                Trip = trip,
                                User = user,
                                Action = (byte)action,
                                CreatedOnUTC = DateTime.UtcNow,
                                TableName = (byte)table,
                                TableId = tableId
                            };
            Add(h);
            Save();
            return h;
        }
    }
}