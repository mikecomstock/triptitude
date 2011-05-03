using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Services
{
    public class WebsiteService
    {
        public Website AddWebsite(string url)
        {
            WebClient x = new WebClient();
            string source = x.DownloadString(url);
            string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            title = HttpUtility.HtmlDecode(title);

            Website website = new Website
            {
                URL = url,
                Title = title
            };

            WebsitesRepo websitesRepo = new WebsitesRepo();
            websitesRepo.Add(website);
            websitesRepo.Save();

            //Util.CreateThumbnails(url, website.Id);

            return website;
        }
    }
}
