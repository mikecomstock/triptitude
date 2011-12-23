using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class PackingController : TriptitudeController
    {
        private readonly TripsRepo tripsRepo;
        private readonly PackingListItemsRepo packingListItemsRepo;

        public PackingController()
        {
            tripsRepo = new TripsRepo();
            packingListItemsRepo = new PackingListItemsRepo();
        }

        public ActionResult Create()
        {
            var form = new PackingItemForm
                           {
                               TripId = CurrentUser.DefaultTrip.Id,
                               Visibility_Id = (int)Visibility.Public
                           };
            ViewBag.Form = form;
            ViewBag.Trip = CurrentUser.DefaultTrip;
            ViewBag.Action = Url.SavePackingItem();
            return PartialView("PackingItemDialog");
        }

        [HttpPost]
        public ActionResult Save(PackingItemForm form)
        {
            var trip = tripsRepo.Find(form.TripId);
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            packingListItemsRepo.Save(form);
            var response = new { status = "OK" };
            return Json(response);
        }

        public ActionResult Edit(int id)
        {
            var packingListItem = packingListItemsRepo.Find(id);
            var trip = packingListItem.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            ViewBag.PackingListItem = packingListItem;
            ViewBag.Form = packingListItemsRepo.GetForm(id);
            ViewBag.Trip = trip;
            ViewBag.Action = Url.SavePackingItem();
            return PartialView("PackingItemDialog");
        }

        public ActionResult Delete(int id)
        {
            var packingListItem = packingListItemsRepo.Find(id);
            var trip = packingListItem.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            packingListItemsRepo.Delete(packingListItem);
            packingListItemsRepo.Save();
            return Redirect(Url.PackingList(trip));
        }
    }
}