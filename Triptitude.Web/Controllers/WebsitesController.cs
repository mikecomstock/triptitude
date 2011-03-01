using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class WebsitesController : Controller
    {
        public ActionResult Details(int id)
        {
            WebsitesRepo websitesRepo = new WebsitesRepo();
            Website website = websitesRepo.Find(id);
            ViewBag.Website = website;
            return View();
        }
    }
}