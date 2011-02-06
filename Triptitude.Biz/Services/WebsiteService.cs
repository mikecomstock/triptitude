using Triptitude.Biz.Models;

namespace Triptitude.Biz.Services
{
    public class WebsiteService
    {
        public void AddWebsite(string url)
        {
            Website website = new Website();
            website.URL = url;
            website.Title = "New website";

            var repo = new Repo<Website>();
            repo.Add(website);
            repo.Save();

            Util.CreateThumbnails(url, website.Id);
        }
    }
}
