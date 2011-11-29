using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Areas.API.Controllers
{
    public class TagsController : Controller
    {
        //
        // GET: /API/Tags/

        public ActionResult Index()
        {
            var tagsRepo = new TagsRepo();
            var tags = tagsRepo.FindAll().Where(t => t.ItemTags.Where(it => it.ShowInSearch).Count() > 0 && t.ShowInSearch).ToList().OrderBy(t => t.NiceName);

            var result = from t in tags
                         select new
                                    {
                                        t.Id,
                                        t.Name,
                                        t.NiceName,
                                        Items = from it in t.ItemTags.Where(it => it.ShowInSearch).OrderBy(it => it.Item.Name)
                                                select new
                                                           {
                                                               it.Item.Id,
                                                               it.Item.Name
                                                           }
                                    };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
