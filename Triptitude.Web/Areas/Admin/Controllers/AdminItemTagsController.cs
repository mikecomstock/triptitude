using System;
using System.Web.Mvc;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Areas.Admin.Controllers
{
    [Authorize(Users = "1|mikecomstock@gmail.com")]
    public class AdminItemTagsController : Controller
    {
        ItemTagRepo itemTagRepo;

        public AdminItemTagsController()
        {
            itemTagRepo = new ItemTagRepo();
        }

        public ActionResult Index(int take = 10)
        {
            ViewBag.Take = take;
            ViewBag.ItemTags = itemTagRepo.FindAll();
            return View();
        }

        public ActionResult Create(string itemName, string tagName)
        {
            var item = new ItemRepo().FindOrInitialize(itemName);
            var tag = new TagsRepo().FindOrInitializeByName(tagName);
            tag.ModeratedOnUTC = DateTime.UtcNow;
            tag.ShowInSearch = true;
            var itemTag = itemTagRepo.FindOrInitialize(item, tag);
            itemTag.ShowInSearch = true;
            itemTag.ModeratedOnUTC = DateTime.UtcNow;
            itemTagRepo.Save();

            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        public ActionResult Moderate(int id, bool show)
        {
            var itemTag = itemTagRepo.Find(id);
            itemTag.ModeratedOnUTC = DateTime.UtcNow;
            itemTag.ShowInSearch = show;
            itemTagRepo.Save();

            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        public ActionResult Delete(int id)
        {
            var itemTag = itemTagRepo.Find(id);
            itemTagRepo.Delete(itemTag);
            itemTagRepo.Save();

            return Redirect(Request.UrlReferrer.AbsolutePath);
        }
    }
}