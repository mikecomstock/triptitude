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

        public ActionResult Header(User currentUser)
        {
            ViewBag.User = currentUser;
            return PartialView();
        }
    }
}
