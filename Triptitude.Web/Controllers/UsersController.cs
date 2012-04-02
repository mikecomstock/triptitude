using System.Web.Mvc;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class UsersController : TriptitudeController
    {
        private UsersRepo repo;
        public UsersController()
        {
            repo = new UsersRepo();
        }

        public ActionResult Details(int id)
        {
            var user = repo.Find(id);
            ViewBag.User = user;
            return View();
        }

        public ActionResult Current()
        {
            return Json(CurrentUser.Json(CurrentUser), JsonRequestBehavior.AllowGet);
        }
    }
}