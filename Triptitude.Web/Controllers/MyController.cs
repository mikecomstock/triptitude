using System.Web.Mvc;

namespace Triptitude.Web.Controllers
{
    public class MyController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Trips()
        {
            return View();
        }
    }
}
