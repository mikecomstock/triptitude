using Triptitude.Biz.Models;
using System.Linq;

namespace Triptitude.Biz.Repos
{
    public class TransportationTypesRepo : Repo<TransportationType>
    {
        public TransportationType Find(string name)
        {
            return FindAll().First(tt => tt.Name == name);
        }
    }
}