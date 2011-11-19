using System.Linq;
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

            ViewBag.Items = new ItemTagRepo().MostPopular(20, tag).Select(it => it.Item).ToList();
            return View();
        }
    }
}