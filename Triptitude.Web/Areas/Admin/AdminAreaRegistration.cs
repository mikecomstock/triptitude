using System.Web.Mvc;

namespace Triptitude.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "Admin"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            MapAdminRoute(context, "trips");
            MapAdminRoute(context, "tags");

            context.MapRoute("Admin_home", "admin/{action}/{id}", new { controller = "adminhome", action = "index", id = UrlParameter.Optional });
        }

        private static void MapAdminRoute(AreaRegistrationContext context, string name)
        {
            context.MapRoute("Admin_" + name, "admin/" + name + "/{action}/{id}", new { controller = "admin" + name, action = "index", id = UrlParameter.Optional });
        }
    }
}
