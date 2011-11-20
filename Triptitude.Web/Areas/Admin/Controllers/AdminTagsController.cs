using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Areas.Admin.Controllers
{
    [Authorize(Users = "1|mikecomstock@gmail.com")]
    public class AdminTagsController : Controller
    {
        private TagsRepo tagsRepo;

        public AdminTagsController()
        {
            tagsRepo = new TagsRepo();
        }

        public ActionResult Index()
        {
            ViewBag.Tags = tagsRepo.FindAll();
            return View();
        }

        public ActionResult Create(string tagName)
        {
            var tag = tagsRepo.FindOrInitializeByName(tagName);
            tagsRepo.Save();
            return RedirectToAction("edit", new { tag.Id });
        }

        public ActionResult Edit(int id)
        {
            Tag tag = tagsRepo.Find(id);
            return View(tag);
        }

        [HttpPost]
        public ActionResult Edit(int id, string name, bool ShowInSearch)
        {
            name = name.Trim().ToLower();

            if (tagsRepo.FindAll().Any(t => t.Name == name && t.Id != id))
            {
                throw new Exception(string.Format("Tag {0} already exists!", name));
            }

            Tag tag = tagsRepo.Find(id);
            tag.Name = name;
            tag.ModeratedOnUTC = DateTime.UtcNow;
            tag.ShowInSearch = ShowInSearch;
            tagsRepo.Save();

            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        public ActionResult Delete(int id)
        {
            Tag tag = tagsRepo.Find(id);
            tagsRepo.Delete(tag);
            tagsRepo.Save();
            return RedirectToAction("index");
        }
    }
}