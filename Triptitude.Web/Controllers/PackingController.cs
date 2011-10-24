using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class PackingController : Controller
    {
        private readonly TripsRepo tripsRepo;
        private readonly PackingListItemsRepo packingListItemsRepo;

        public PackingController()
        {
            tripsRepo = new TripsRepo();
            packingListItemsRepo = new PackingListItemsRepo();
        }

        public ActionResult Create(User currentUser)
        {
            var form = new PackingItemForm
                           {
                               TripId = currentUser.DefaultTrip.Id,
                               Public = true
                           };
            ViewBag.Form = form;
            ViewBag.Trip = currentUser.DefaultTrip;
            ViewBag.Action = Url.CreatePackingItem();
            return PartialView("PackingItemDialog");
        }

        [HttpPost]
        public ActionResult Create(PackingItemForm form, User currentUser)
        {
            var trip = tripsRepo.Find(form.TripId);
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

            packingListItemsRepo.Save(form);
            return Redirect(Url.PackingList(trip));
        }

        public ActionResult Edit(int id, User currentUser)
        {
            var packingListItem = packingListItemsRepo.Find(id);
            var trip = packingListItem.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

            ViewBag.PackingListItem = packingListItem;
            ViewBag.Form = packingListItemsRepo.GetForm(id);
            ViewBag.Trip = trip;
            ViewBag.Action = Url.CreatePackingItem();
            return PartialView("PackingItemDialog");
        }

        public ActionResult Delete(int id, User currentUser)
        {
            var packingListItem = packingListItemsRepo.Find(id);
            var trip = packingListItem.Trip;
            bool userOwnsTrip = PermissionHelper.UserOwnsTrips(currentUser, trip);
            if (!userOwnsTrip) return PartialView("WorkingOnIt");// Redirect("/");

            packingListItemsRepo.Delete(packingListItem);
            packingListItemsRepo.Save();
            return Redirect(Url.PackingList(trip));
        }
    }
}