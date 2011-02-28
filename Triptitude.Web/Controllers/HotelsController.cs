using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class HotelsController : Controller
    {
        public ActionResult Index()
        {
            IQueryable<ExpediaHotel> hotels = new ExpediaHotelsRepo().FindAll().Take(15);
            ViewBag.Hotels = hotels;
            return View();
        }

        public ActionResult Details(int id)
        {
            ExpediaHotelsRepo expediaHotelsRepo = new ExpediaHotelsRepo();
            ExpediaHotel hotel = expediaHotelsRepo.FindByBaseItemId(id);
            ViewBag.Hotel = hotel;
            return View();
        }
    }
}
