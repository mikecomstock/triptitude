using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class MapsController : Controller
    {
        public ActionResult Hotel(int id)
        {
            ExpediaHotelsRepo expediaHotelsRepo = new ExpediaHotelsRepo();
            ExpediaHotel expediaHotel = expediaHotelsRepo.FindByBaseItemId(id);
            ViewBag.ExpediaHotel = expediaHotel;
            return View();
        }
    }
}
