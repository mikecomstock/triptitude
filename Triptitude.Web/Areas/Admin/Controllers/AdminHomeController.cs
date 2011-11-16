using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Areas.Admin.Controllers
{
    [Authorize(Users = "1|mikecomstock@gmail.com")]
    public class AdminHomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ResetAllActivityTags()
        {
            var activitiesRepo = new ActivitiesRepo();
            var activities = activitiesRepo.FindAll();
            foreach (var activity in activities)
            {
                if (activity.Tags != null) activity.Tags.Clear();
                if (!string.IsNullOrWhiteSpace(activity.TagString))
                {
                    activity.Tags = new TagsRepo().FindOrInitializeAll(activity.TagString).ToList();
                }
            }

            activitiesRepo.Save();

            var packingListItemsRepo = new PackingListItemsRepo();
            var itemTagRepo = new ItemTagRepo();
            var packingListItems = packingListItemsRepo.FindAll();
            foreach (var packingListItem in packingListItems)
            {
                Tag tag = null;
                if (!string.IsNullOrWhiteSpace(packingListItem.TagString))
                {
                    tag = new TagsRepo().FindOrInitializeAll(packingListItem.TagString).First();
                }


                var itemTag = itemTagRepo.FindOrInitialize(packingListItem.ItemTag.Item, tag);
                packingListItem.ItemTag = itemTag;
            }

            packingListItemsRepo.Save();

            var itemTagsToDelete = itemTagRepo.FindAll().Where(it => it.PackingListItems.Count() == 0);
            foreach (var itemTag in itemTagsToDelete)
            {
                itemTagRepo.Delete(itemTag);
            }
            itemTagRepo.Save();

            return Redirect(Url.Admin());
        }
    }
}