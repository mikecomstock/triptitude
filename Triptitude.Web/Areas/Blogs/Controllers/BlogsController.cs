using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Areas.Blogs.Controllers
{
    public class BlogsController : Controller
    {
        public ActionResult Index()
        {
            return RedirectPermanent(Url.BlogsCategory("tech"));
            return View();
        }

        public ActionResult Category(string category)
        {
            ViewBag.Category = category;

            IQueryable<BlogPost> posts = new BlogPostsRepo().FindByCategory(category);
            ViewBag.Posts = posts;
            return View();
        }


        public ActionResult Details(string category, int id)
        {
            var post = new BlogPostsRepo().Find(id);
            ViewBag.Post = post;
            return View();
        }
    }
}
