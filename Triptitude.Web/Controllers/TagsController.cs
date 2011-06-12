using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class TagsController : Controller
    {
        private TagsRepo repo;
        public TagsController()
        {
            repo = new TagsRepo();
        }

        public ActionResult Details(int id)
        {
            Tag tag = repo.Find(id);
            ViewBag.Tag = tag;
            return View();
        }
    }
}