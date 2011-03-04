using System;
using System.Linq;
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

            Random r = new Random();
            IQueryable<ExpediaHotel> nearbyHotels = expediaHotelsRepo.FindAll().OrderBy(h => h.BaseItem.Name).Skip(r.Next(0, 100)).Take(10);
            ViewBag.NearbyHotels = nearbyHotels;

            return View();
        }
    }
}
