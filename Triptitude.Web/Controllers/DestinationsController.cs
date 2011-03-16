using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class DestinationsController : Controller
    {
        public ActionResult City(int id)
        {
            var citiesRepo = new CitiesRepo();
            City city = citiesRepo.Find(id);
            ViewBag.City = city;
            ViewBag.Trips = new TripsRepo().FindAll().Take(5);
            return View();
        }
    }
}
