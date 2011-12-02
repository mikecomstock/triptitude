using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class HotelsController : Controller
    {
        public ActionResult Details(int id)
        {
            HotelsRepo hotelsRepo = new HotelsRepo();
            Hotel hotel = hotelsRepo.Find(id);
            if (hotel == null) return HttpNotFound();
            ViewBag.Hotel = hotel;
            return View();
        }
    }
}
