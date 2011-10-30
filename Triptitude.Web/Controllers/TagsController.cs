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

            ViewBag.Items = (from it in tag.ItemTags
                             orderby it.PackingListItems.Count() descending, it.Item.Name
                             where it.ShowInSite && it.PackingListItems.Count() > 0
                             select it.Item).Take(20).ToList();

            return View();
        }
    }
}