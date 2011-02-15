using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class ItineraryItemsController : Controller
    {
        public ActionResult Edit(int id)
        {
            var itineraryItemsRepo = new ItineraryItemsRepo();
            var itineraryItem = itineraryItemsRepo.Find(id);
            var itineraryItemSettings = itineraryItemsRepo.GetSettings(itineraryItem);

            ViewBag.ItineraryItem = itineraryItem;
            ViewBag.Settings = itineraryItemSettings;

            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, ItineraryItemSettings settings)
        {
            var itineraryItemsRepo = new ItineraryItemsRepo();
            ItineraryItem itineraryItem = itineraryItemsRepo.Find(id);
            itineraryItemsRepo.Save(itineraryItem, settings);
            return Redirect(Url.TripDetails(itineraryItem.Trip));
        }
    }
}