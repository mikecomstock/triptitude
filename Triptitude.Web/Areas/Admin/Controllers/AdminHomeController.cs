using System.Linq;
using System.Web.Mvc;
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

            return Redirect(Url.Admin());
        }
    }
}