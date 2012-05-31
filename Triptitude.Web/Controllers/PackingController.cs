using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class PackingController : TriptitudeController
    {
        private readonly TripsRepo tripsRepo;
        private readonly PackingListItemsRepo packingListItemsRepo;
        private readonly Repo<PackingItem> packingItemRepo;

        public PackingController()
        {
            tripsRepo = new TripsRepo();
            packingListItemsRepo = new PackingListItemsRepo();
            packingItemRepo = new Repo<PackingItem>();
        }

        public ActionResult Index()
        {
            ViewBag.CurrentUser = CurrentUser;
            var tags = packingListItemsRepo.FindAll()
                .Where(pli => pli.ItemTag.ShowInSearch)
                .GroupBy(pli => pli.ItemTag.Tag)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Where(t => t.ShowInSearch);

            ViewBag.Tags = tags;
            return View();
        }

        public ActionResult Create(string tag, string item, int quantity)
        {
            var trip = CurrentUser.DefaultTrip;
            var packingItem = trip.PackingItems.FirstOrDefault(pi => pi.Item == item.Trim() && pi.Tag == tag.Trim());
            if (packingItem == null)
            {
                packingItem = new PackingItem
                                     {
                                         Trip = CurrentUser.DefaultTrip,
                                         CreatedUser = CurrentUser,
                                         ForUser = CurrentUser,
                                         Created_At = DateTime.UtcNow,
                                         Item = item.Trim(),
                                         Tag = tag.Trim()
                                     };
                trip.PackingItems.Add(packingItem);
            }

            packingItem.Quantity = quantity;
            packingItem.Modified_At = DateTime.UtcNow;

            if (packingItem.Quantity <= 0)
            {
                packingItem.Quantity = 0;
                packingItemRepo.Delete(packingItem);
            }

            tripsRepo.Save();

            return Json(packingItem.Json(CurrentUser));
        }

        //public ActionResult Create()
        //{
        //    var form = new PackingItemForm
        //                   {
        //                       TripId = CurrentUser.DefaultTrip.Id,
        //                       Visibility_Id = (int)Visibility.Public
        //                   };
        //    ViewBag.Form = form;
        //    ViewBag.Trip = CurrentUser.DefaultTrip;
        //    ViewBag.Action = Url.SavePackingItem();
        //    return PartialView("PackingItemDialog");
        //}

        //[HttpPost]
        //public ActionResult Save(PackingItemForm form)
        //{
        //    var trip = tripsRepo.Find(form.TripId);
        //    if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

        //    PackingListItem packingListItem = packingListItemsRepo.Save(form);

        //    //new HistoriesRepo().Create(CurrentUser, trip, form.PackingItemId.HasValue ? HistoryAction.Modified : HistoryAction.Created, HistoryTable.PackingListItems, packingListItem.Id);

        //    var response = new { status = "OK" };
        //    return Json(response);
        //}

        //public ActionResult Edit(int id)
        //{
        //    var packingListItem = packingListItemsRepo.Find(id);
        //    var trip = packingListItem.Trip;
        //    if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

        //    ViewBag.PackingListItem = packingListItem;
        //    ViewBag.Form = packingListItemsRepo.GetForm(id);
        //    ViewBag.Trip = trip;
        //    ViewBag.Action = Url.SavePackingItem();
        //    return PartialView("PackingItemDialog");
        //}

        //public ActionResult Delete(int id)
        //{
        //    var packingListItem = packingListItemsRepo.Find(id);
        //    var trip = packingListItem.Trip;
        //    if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

        //    packingListItemsRepo.Delete(packingListItem);
        //    packingListItemsRepo.Save();

        //    //new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Deleted, HistoryTable.PackingListItems, packingListItem.Id);

        //    return Redirect(Url.PackingList(trip));
        //}
    }
}