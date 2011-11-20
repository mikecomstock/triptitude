using System;
using System.Web.Mvc;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

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

        public ActionResult Create(string itemName, string tagName, int take = 10)
        {
            var item = new ItemRepo().FindOrInitialize(itemName);
            var tag = new TagsRepo().FindOrInitializeByName(tagName);
            var itemTag = itemTagRepo.FindOrInitialize(item, tag);
            itemTag.ShowInSite = true;
            itemTag.ModeratedOnUTC = DateTime.UtcNow;
            itemTagRepo.Save();
            return Redirect(Url.Admin("itemtags", "index", new { take }));
        }

        public ActionResult Moderate(int id, bool show, int take = 10)
        {
            var itemTag = itemTagRepo.Find(id);
            itemTag.ModeratedOnUTC = DateTime.UtcNow;
            itemTag.ShowInSite = show;
            itemTagRepo.Save();

            return Redirect(Url.Admin("itemtags", "index", new { take }));
        }
    }
}