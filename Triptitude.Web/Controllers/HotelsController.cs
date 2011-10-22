using System.Web.Mvc;
using Triptitude.Biz.Forms;
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
            ViewBag.Hotel = hotel;
            return View();
        }

        public ActionResult Search(HotelSearchForm form)
        {
            ViewBag.Hotels = new HotelsRepo().Search(form);
            return PartialView();
        }
    }
}
