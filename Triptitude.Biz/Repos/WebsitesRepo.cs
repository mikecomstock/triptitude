using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class WebsitesRepo : Repo<WebsiteActivity>
    {
        public WebsiteActivity FindByUrl(string url)
        {
            return FindAll().FirstOrDefault(w => w.URL == url);
        }
    }
}