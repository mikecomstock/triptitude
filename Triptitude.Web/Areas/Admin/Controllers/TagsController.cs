using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Areas.Admin.Controllers
{ 
    public class TagsController : Controller
    {
        private Db db = new Db();

        //
        // GET: /Admin/Tags/

        public ViewResult Index()
        {
            return View(db.Tags.ToList());
        }

        //
        // GET: /Admin/Tags/Details/5

        public ViewResult Details(int id)
        {
            Tag tag = db.Tags.Find(id);
            return View(tag);
        }

        //
        // GET: /Admin/Tags/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Tags/Create

        [HttpPost]
        public ActionResult Create(Tag tag)
        {
            if (ModelState.IsValid)
            {
                db.Tags.Add(tag);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(tag);
        }
        
        //
        // GET: /Admin/Tags/Edit/5
 
        public ActionResult Edit(int id)
        {
            Tag tag = db.Tags.Find(id);
            return View(tag);
        }

        //
        // POST: /Admin/Tags/Edit/5

        [HttpPost]
        public ActionResult Edit(Tag tag)
        {
            if (ModelState.IsValid)
            {
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tag);
        }

        //
        // GET: /Admin/Tags/Delete/5
 
        public ActionResult Delete(int id)
        {
            Tag tag = db.Tags.Find(id);
            return View(tag);
        }

        //
        // POST: /Admin/Tags/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Tag tag = db.Tags.Find(id);
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