using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class ItineraryItemsController : Controller
    {
        private ItineraryItemsRepo itineraryItemsRepo;

        public ItineraryItemsController()
        {
            itineraryItemsRepo = new ItineraryItemsRepo();
        }

        public ActionResult Edit(int id)
        {
            var itineraryItem = itineraryItemsRepo.Find(id);
            var itineraryItemSettings = itineraryItemsRepo.GetSettings(itineraryItem);

            ViewBag.ItineraryItem = itineraryItem;
            ViewBag.Settings = itineraryItemSettings;

            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, ItineraryItemSettings settings)
        {
            ItineraryItem itineraryItem = itineraryItemsRepo.Find(id);
            itineraryItemsRepo.Save(itineraryItem, settings);
            return Redirect(Url.PlanItinerary(itineraryItem.Trip));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            ItineraryItem itineraryItem = itineraryItemsRepo.Find(id);
            itineraryItemsRepo.SoftDelete(itineraryItem);
            return Redirect(Url.PlanItinerary(itineraryItem.Trip));
        }


        [HttpPost]
        public ActionResult AddWebsiteToTrip(string url, User currentUser)
        {
            Website website = new WebsiteService().AddWebsite(url);
            var itineraryItem = itineraryItemsRepo.AddWebsiteToTrip(website, currentUser.DefaultTrip);
            return Redirect(Url.EditItineraryItem(itineraryItem));
        }

        public ActionResult AddBaseItemToTrip(int baseItemId, User currentUser)
        {
            BaseItemsRepo baseItemsRepo = new BaseItemsRepo();
            BaseItem baseItem = baseItemsRepo.Find(baseItemId);
            ItineraryItem itineraryItem = itineraryItemsRepo.AddBaseItemToTrip(baseItem, currentUser.DefaultTrip);
            return Redirect(Url.EditItineraryItem(itineraryItem));
        }
    }
}