using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

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
            ViewBag.CurrentUser = currentUser;
            ViewBag.HideTripBar = hideTripBar;
            return PartialView();
        }

        //public ActionResult Signup()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Signup(SignupForm form)
        //{
        //    Dictionary<string, string> d = new Dictionary<string, string>();
        //    foreach (var key in HttpContext.Request.ServerVariables.AllKeys)
        //    {
        //        d.Add(key, HttpContext.Request.ServerVariables[key]);
        //    }
        //    string json = new JavaScriptSerializer().Serialize(d);
            
        //    SignupRepo signupRepo = new SignupRepo();
        //    string ip = GetIPAddress(HttpContext);
        //    signupRepo.Save(form, ip, json);
        //    return Redirect(Url.Signup());
        //}

        //string GetIPAddress(HttpContextBase context)
        //{
        //    string ip;

        //    if (!string.IsNullOrWhiteSpace(context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
        //    {
        //        ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //        ip = string.IsNullOrWhiteSpace(ip) ? null: ip.Split(',')[0];
        //    }
        //    else
        //    {
        //        ip = context.Request.ServerVariables["REMOTE_ADDR"];
        //    }

        //    return ip;
        //}

        public ActionResult Sitemap()
        {
            TripsRepo tripsRepo = new TripsRepo();
            IQueryable<Trip> trips = tripsRepo.FindAll().Where(t => t.ShowInSiteMap);
            ViewBag.Trips = trips;

            Response.ContentType = "text/xml";
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 410;
            return View();
        }
    }
}
