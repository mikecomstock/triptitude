using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class ExpediaHotelsRepo : Repo<ExpediaHotel>
    {
        public ExpediaHotel FindByBaseItemId(int id)
        {
            return FindAll().FirstOrDefault(h => h.BaseItem.Id == id);
        }
    }
}