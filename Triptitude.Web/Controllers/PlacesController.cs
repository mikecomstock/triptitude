using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class PlacesController : Controller
    {
        private PlacesRepo placesRepo;
        public PlacesController()
        {
            placesRepo = new PlacesRepo();
        }

        //public ActionResult Search(PlaceSearchForm form)
        //{
        //    var placesService = new PlacesService();
        //    IEnumerable<Place> places = placesService.Search(form).ToList();
        //    ViewBag.Places = places;
        //    return PartialView();
        //}

        [HttpPost]
        public ActionResult Search(string googReference)
        {
            Place place = placesRepo.FindOrCreateByGoogReference(googReference);
            return Redirect(Url.Details(place));
        }

        public ActionResult Details(int id)
        {
            var place = placesRepo.Find(id);
            ViewBag.Place = place;
            return View();
        }

        public ActionResult Nearby(int id)
        {
            var place = placesRepo.Find(id);
            ViewBag.Place = place;
            return View();
        }
    }
}