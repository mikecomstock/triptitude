using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Services;
using AmazonItem = Triptitude.Biz.Models.AmazonItem;

namespace Triptitude.Web.Areas.Admin.Controllers
{
    [Authorize(Users = "1|mikecomstock@gmail.com")]
    public class AdminTagsController : Controller
    {
        private readonly Db db = new Db();

        public ViewResult Index()
        {
            return View(db.Tags.OrderBy(t => t.Name));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Tag tag)
        {
            if (ModelState.IsValid)
            {
                db.Tags.Add(tag);
                db.SaveChanges();
                return RedirectToAction("edit", new { tag.Id });
            }

            return View(tag);
        }

        public ActionResult Edit(int id)
        {
            Tag tag = db.Tags.Find(id);
            return View(tag);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection form)
        {
            Tag t = db.Tags.Find(id);
            t.Name = form["name"];

            t.AmazonItems.Clear();
            if (!string.IsNullOrWhiteSpace(form["items"]))
            {
                var itemIDs = form["items"].Split(',').Select(i => int.Parse(i));
                foreach (var itemId in itemIDs)
                {
                    t.AmazonItems.Add(db.AmazonItems.Find(itemId));
                }
            }
            db.SaveChanges();

            if (!string.IsNullOrWhiteSpace(form["newitem.name"]))
            {
                AmazonItem i = new AmazonItem
                             {
                                 Name = form["newitem.Name"].Trim(),
                                 DetailPageURL = form["newitem.DetailPageURL"].Trim(),
                                 SmallImageURL = form["newitem.SmallImageURL"].Trim(),
                                 ASIN = form["newitem.ASIN"].Trim()
                             };

                if (!string.IsNullOrWhiteSpace(i.ASIN))
                {
                    var amazonService = new AmazonService();
                    var product = amazonService.Find(i.ASIN);
                    i.DetailPageURL = product.DetailPageURL;
                    i.SmallImageURL = product.SmallImageURL;
                    i.SmallImageHeight = product.SmallImageHeight;
                    i.SmallImageWidth = product.SmallImageWidth;
                }

                t.AmazonItems.Add(i);
                db.SaveChanges();
                return RedirectToAction("edit", new { id });
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        public ActionResult Delete(int id)
        {
            Tag tag = db.Tags.Find(id);
            tag.AmazonItems.Clear();
            db.Tags.Remove(tag);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}