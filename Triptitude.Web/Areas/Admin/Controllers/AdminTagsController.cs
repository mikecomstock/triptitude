using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;

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

            t.Items.Clear();
            if (!string.IsNullOrWhiteSpace(form["items"]))
            {
                var itemIDs = form["items"].Split(',').Select(i => int.Parse(i));
                foreach (var itemId in itemIDs)
                {
                    t.Items.Add(db.Items.Find(itemId));
                }
            }
            db.SaveChanges();

            if (!string.IsNullOrWhiteSpace(form["newitem.name"]))
            {
                Item i = new Item
                             {
                                 Name = form["newitem.Name"],
                                 LinkURL = form["newitem.LinkURL"],
                                 ImageURL = form["newitem.ImageURL"],
                                 ASIN = form["newitem.ASIN"]
                             };
                t.Items.Add(i);
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
            tag.Items.Clear();
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