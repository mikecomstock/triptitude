using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Areas.Admin.Controllers
{
    [Authorize(Users = "1|mikecomstock@gmail.com")]
    public class AdminHomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Trips = new TripsRepo().FindAll();
            ViewBag.Tags = new TagsRepo().FindAll();
            ViewBag.ItemTags = new ItemTagRepo().FindAll();
            return View();
        }

        // http://localhost:50025/admin/adminhome/FixActivityPlaces
        //public ActionResult FixActivityPlaces()
        //{
        //    var activitiesRepo = new ActivitiesRepo();
        //    var activityPlacesRepo = new ActivityPlacesRepo();

        //    var activities = activitiesRepo.FindAll().ToList();

        //    foreach (var activity in activities.OfType<TransportationActivity>())
        //    {
        //        if (activity.FromPlace != null) activityPlacesRepo.FindOrInitialize(activity, 0, activity.FromPlace);
        //        if (activity.ToPlace != null) activityPlacesRepo.FindOrInitialize(activity, 1, activity.ToPlace);
        //    }

        //    foreach (var activity in activities.OfType<PlaceActivity>())
        //    {
        //        if (activity.Place != null) activityPlacesRepo.FindOrInitialize(activity, 0, activity.Place);
        //    }

        //    activitiesRepo.Save();

        //    return Content("Done");
        //}

        // This is pretty bad.. very slow because of all the .Save()ing going on.
        //public ActionResult ResetAllActivityTags()
        //{
        //    var tagsRepo = new TagsRepo();

        //    var activitiesRepo = new ActivitiesRepo();
        //    var activities = activitiesRepo.FindAll().ToList();
        //    foreach (var activity in activities)
        //    {
        //        if (activity.Tags != null) activity.Tags.Clear();
        //        if (!string.IsNullOrWhiteSpace(activity.TagString))
        //        {
        //            activity.Tags = tagsRepo.FindOrInitializeAll(activity.TagString).ToList();
        //        }
        //        activitiesRepo.Save();
        //    }

        //    activitiesRepo.Save();

        //    var packingListItemsRepo = new PackingListItemsRepo();
        //    var itemTagRepo = new ItemTagRepo();
        //    var packingListItems = packingListItemsRepo.FindAll().ToList();
        //    foreach (var packingListItem in packingListItems)
        //    {
        //        Tag tag = null;
        //        if (!string.IsNullOrWhiteSpace(packingListItem.TagString))
        //        {
        //            tag = tagsRepo.FindOrInitializeAll(packingListItem.TagString).First();
        //        }

        //        var itemTag = itemTagRepo.FindOrInitialize(packingListItem.ItemTag.Item, tag);
        //        packingListItem.ItemTag = itemTag;
        //        packingListItemsRepo.Save();
        //    }

        //    packingListItemsRepo.Save();

        //    var itemTagsToDelete = itemTagRepo.FindAll().Where(it => it.PackingListItems.Count() == 0).ToList();
        //    foreach (var itemTag in itemTagsToDelete)
        //    {
        //        itemTagRepo.Delete(itemTag);
        //    }
        //    itemTagRepo.Save();

        //    return Redirect(Url.Admin());
        //}
    }
}