using System.Web.Mvc;
using System.Web.Security;

namespace Triptitude.Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {     
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~");
        }
    }
}
