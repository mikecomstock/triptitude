using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class TagsController : TriptitudeController
    {
        private TagsRepo repo;
        public TagsController()
        {
            repo = new TagsRepo();
        }

        public ActionResult Details(int id)
        {
            Tag tag = repo.Find(id);
            if (tag == null) return HttpNotFound();

            ViewBag.Tag = tag;
            ViewBag.Items = new ItemTagRepo().MostPopular(20, tag).Select(it => it.Item).ToList();
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        public ActionResult Search(string term)
        {
            var tags = repo.FindAll().Where(t => t.ShowInSearch);
            var result = tags.Where(t => t.Name.StartsWith(term)).OrderBy(t => t.Name).Take(5).ToList().Select(t => t.NiceName).ToArray();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}