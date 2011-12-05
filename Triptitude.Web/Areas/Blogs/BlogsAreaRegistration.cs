using System.Web.Mvc;
using System.Web.Routing;

namespace Triptitude.Web.Areas.Blogs
{
    public class BlogsAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "Blogs"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Names are important for some of these, they are used in UrlHelperExtensions.
            context.MapRoute("blogs_index", "blogs", new { controller = "blogs", action = "index" });
            context.MapRoute("blogs_category", "blogs/{category}", new { controller = "blogs", action = "category" });
            context.Routes.MapSlugRoute("blogs_details", "blogs/{category}/{idslug}", new { controller = "blogs", action = "details" }, new { idslug = new SlugRouteConstraint() }, new { Area = "Blogs" });

            // Needed for Html.RenderAction
            context.MapRoute("blogs_content", "blogs/{category}/{id}", new { controller = "blogs", action = "content" });
        }
    }
}
