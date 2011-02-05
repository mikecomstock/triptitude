using System.Web.Mvc;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Users = new Repo().FindAll<User>();
            return View();
        }
    }
}
