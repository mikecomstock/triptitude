using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Triptitude.Biz.Models;
using Triptitude.Web.ModelBinders;

namespace Triptitude.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterModelBinders()
        {
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(User), new CurrentUserModelBinder());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Login", "Login", new { controller = "Auth", action = "Login" });
            routes.MapRoute("Logout", "Logout", new { controller = "Auth", action = "Logout" });

            routes.MapRoute("Details", "{controller}/{id}/{title}/{action}", new { action = "Details", title = UrlParameter.Optional }, new { id = new IntegerRouteConstraint() });
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterModelBinders();
            RegisterRoutes(RouteTable.Routes);
        }
    }

    public class IntegerRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string id = values[parameterName].ToString();

            int i;
            return int.TryParse(id, out i);
        }
    }

}