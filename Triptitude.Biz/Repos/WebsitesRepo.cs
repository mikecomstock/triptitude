using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class WebsitesRepo : Repo<Website>
    {
        public Website FindByUrl(string url)
        {
            return FindAll().FirstOrDefault(w => w.URL == url);
        }
    }
}