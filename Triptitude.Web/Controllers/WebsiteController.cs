using System.Web.Mvc;
using Triptitude.Biz.Services;

namespace Triptitude.Web.Controllers
{
    public class WebsiteController : Controller
    {
        [HttpPost]
        public ActionResult Create(string url)
        {
            new WebsiteService().CreateThumbnailAsync(url);
            return Redirect("~");
        }
    }
}