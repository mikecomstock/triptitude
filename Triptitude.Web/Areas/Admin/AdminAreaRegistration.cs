using System.Web.Mvc;

namespace Triptitude.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "Admin"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("admin_default", "admin/{controller}/{action}/{id}", new { controller = "adminhome", action = "index", id = UrlParameter.Optional });
        }
    }
}
