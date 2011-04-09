using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class WebsitesController : Controller
    {
        private WebsitesRepo websitesRepo;

        public WebsitesController()
        {
            websitesRepo = new WebsitesRepo();
        }

        public ActionResult Details(int id)
        {
            Website website = websitesRepo.Find(id);
            ViewBag.Website = website;
            return View();
        }
    }
}