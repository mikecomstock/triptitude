using System.Web.Mvc;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Controllers
{
    public class MyController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Trips(User currentUser)
        {
            ViewBag.User = currentUser;
            return View();
        }
    }
}
