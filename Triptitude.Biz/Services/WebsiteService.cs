using System.Net;
using System.Text.RegularExpressions;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Services
{
    public class WebsiteService
    {
        public Website AddWebsite(string url)
        {
            WebClient x = new WebClient();
            string source = x.DownloadString(url);
            string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

            Website website = new Website();
            website.URL = url;
            website.Title = title;

            var repo = new Repo<Website>();
            repo.Add(website);
            
            DbProvider.Save();

            Util.CreateThumbnails(url, website.Id);

            return website;
        }
    }
}
