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
        public ActionResult Search(string googReference, string googId)
        {
            return Redirect(googReference, googId);
        }

        public ActionResult Redirect(string googReference, string googId)
        {
            Place place = placesRepo.FindOrCreateByGoogReference(googId, googReference);
            return Redirect(Url.Details(place));
        }

        public ActionResult Details(int id, User currentUser)
        {
            var place = placesRepo.Find(id);
            ViewBag.Place = place;
            ViewBag.CurrentUser = currentUser;
            return View();
        }

        public ActionResult Nearby(int id, User currentUser)
        {
            var place = placesRepo.Find(id);
            return Redirect(Url.Nearby(place));
            ViewBag.Place = place;
            ViewBag.CurrentUser = currentUser;
            return View();
        }
    }
}