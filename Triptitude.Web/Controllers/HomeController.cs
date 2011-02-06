using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Users = new Repo<User>().FindAll();
            ViewBag.Trips = new TripsRepo().FindAll();
            return View();
        }

        public ActionResult Header(User currentUser)
        {
            ViewBag.User = currentUser;
            return PartialView();
        }
    }
}
