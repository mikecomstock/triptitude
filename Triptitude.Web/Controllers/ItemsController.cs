using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class ItemsController : Controller
    {
        public ActionResult Search(string term)
        {
            int take = 7;
            var possibilities = new ItemTagRepo().FindAll().Where(it => it.ShowInSearch);
            var startsWith = possibilities.Where(it => it.Item.Name.StartsWith(term)).Select(it => it.Item.Name).Take(take).ToList();
            var contains = possibilities.Where(it => it.Item.Name.Contains(term)).Select(it => it.Item.Name).Take(take).ToList();
            var combined = startsWith.Union(contains).Take(take);
            return Json(combined, JsonRequestBehavior.AllowGet);
        }
    }
}