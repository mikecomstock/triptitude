using System.Web.Mvc;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class UsersController : TriptitudeController
    {
        public ActionResult Details(int id)
        {
            ViewBag.User = new UsersRepo().Find(id);
            return View();
        }
    }
}