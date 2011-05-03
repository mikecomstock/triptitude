using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;
using Triptitude.Biz.Models;
using Triptitude.Web.ModelBinders;

namespace Triptitude.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ElmahHandleErrorAttribute());
        }

        public static void RegisterModelBinders()
        {
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(User), new CurrentUserModelBinder());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            // Old stuff that's gone
            routes.MapRoute("OldQuestions", "Questions/{id}/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldQuestions2", "Questions", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldTags", "Tag/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldIsers", "Users/{id}/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldHotels", "Hotels/{id}/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldHotels2", "Hotels", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldActivities", "Activities/{id}/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldActivities2", "Activities", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldTrips", "Trips/1/boston-in-a-day/map", new { controller = "Home", Action = "NotFound" });
            // End of Old Stuff


            routes.MapRoute("Sitemap", "sitemap.xml", new { controller = "home", action = "sitemap" });
            routes.MapRoute("Login", "login", new { controller = "auth", action = "login" });
            routes.MapRoute("Logout", "logout", new { controller = "auth", action = "logout" });
            routes.MapRoute("Signup", "signup", new { controller = "home", action = "signup" });

            routes.MapSlugRoute("Details", "{controller}/{idslug}", new { action = "details" }, new { idslug = new SlugRouteConstraint() });
            routes.MapSlugRoute("Slug", "{controller}/{idslug}/{action}", null, new { idslug = new SlugRouteConstraint() });

            routes.MapRoute("DestinationsRedirect", "Destinations/{id}/{name}", new { controller = "destinations", action = "redirect" });

            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "home", action = "index", id = UrlParameter.Optional });
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterModelBinders();
            RegisterRoutes(RouteTable.Routes);
        }
    }

    public class SlugRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string idslug = (string)values[parameterName];
            //if (!idslug.Contains("-")) return false;

            string id = idslug.Split('-')[0];

            int i;
            return int.TryParse(id, out i);
        }
    }

    public class SlugRoute : Route
    {
        public SlugRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler) { }
        public SlugRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler) { }
        public SlugRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler) { }
        public SlugRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler) { }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData data = base.GetRouteData(httpContext);
            if (data == null) return null;

            if (data.Values.ContainsKey("idslug"))
            {
                string idslug = (string)data.Values["idslug"];
                string id = idslug.Split('-')[0];
                data.Values.Add("id", id);
            }

            return data;
        }
    }

    public static class RouteCollectionExtensionHelper
    {
        public static Route MapSlugRoute(this RouteCollection routes, string name, string url, object defaults, object constraints)
        {
            var route = new SlugRoute(url,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(constraints),
                new MvcRouteHandler());
            routes.Add(name, route);
            return route;
        }
    }

    public class ElmahHandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            if (context.ExceptionHandled) // this check somehow makes sure we only get 1 exception email while in dev
                RaiseErrorSignal(context.Exception);
        }

        private static void RaiseErrorSignal(Exception e)
        {
            var context = HttpContext.Current;
            ErrorSignal.FromContext(context).Raise(e, context);
        }
    }

}