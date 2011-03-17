using System.Web.Mvc;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Header(User currentUser, bool? hideTripBar)
        {
            ViewBag.User = currentUser;
            ViewBag.HideTripBar = hideTripBar;
            return PartialView();
        }

        public ActionResult Signup()
        {
            return View();
        }

        public ActionResult Search(string s)
        {
            ViewBag.SearchString = s;
            return PartialView("_SearchResults");
        }
    }
}
