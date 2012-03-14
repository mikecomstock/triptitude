using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Profile;
using System.Web.Routing;
using System.Web.Security;
using Elmah;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
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
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });


            // Old stuff that's gone
            routes.MapRoute("OldQuestions", "Questions/{id}/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldQuestions2", "Questions", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldTags", "Tag/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldIsers", "Users/{id}/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldHotels", "Hotels/{id}/{name}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldHotels2", "Hotels", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldHotels3", "Hotels/{*data}", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldTrips", "Trips/1/boston-in-a-day/map", new { controller = "Home", Action = "NotFound" });
            routes.MapRoute("OldDestinations", "Destinations/{*Data}", new { controller = "Home", Action = "NotFound" });
            // End of Old Stuff

            routes.MapRoute("Sitemap", "sitemap.xml", new { controller = "home", action = "sitemap" });
            routes.MapRoute("Login", "login", new { controller = "auth", action = "login" });
            routes.MapRoute("Logout", "logout", new { controller = "auth", action = "logout" });
            routes.MapRoute("ForgotPass", "forgotpass", new { controller = "auth", action = "forgotpass" });

            routes.MapSlugRoute("Details", "{controller}/{idslug}", new { action = "details" }, new { idslug = new SlugRouteConstraint() }, new { namespaces = new[] { "Triptitude.Web.Controllers" } });
            routes.MapSlugRoute("Slug", "{controller}/{idslug}/{action}", null, new { idslug = new SlugRouteConstraint() }, new { namespaces = new[] { "Triptitude.Web.Controllers" } });

            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "home", action = "index", id = UrlParameter.Optional }, new string[] { "Triptitude.Web.Controllers" });
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterModelBinders();
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.Add(new DynamicFolderBundle("css", new CssMinify(), "*.css", true));

            Bundle js = new Bundle("~/Scripts/js");
            js.AddFile("~/Scripts/jquery-1.7.1.min.js");
            js.AddFile("~/Scripts/jquery-ui-1.8.18.min.js");
            js.AddFile("~/Scripts/underscore.min.js");
            js.AddFile("~/Scripts/backbone.min.js");
            js.AddFile("~/Scripts/jquery.pjax.js");
            js.AddFile("~/Scripts/jquery.placeholder.js");

            js.AddFile("~/Scripts/app/triptitude.js");
            js.AddFile("~/Scripts/app/T.js");
            js.AddDirectory("~/Scripts/app", "*.js", true);
            BundleTable.Bundles.Add(js);
        }

        public void Profile_OnMigrateAnonymous(object sender, ProfileMigrateEventArgs args)
        {
            int userId = int.Parse(args.Context.User.Identity.Name.Split('|')[0]);
            new UsersRepo().MigrateAnonymousUser(args.AnonymousID, userId);
            AnonymousIdentificationModule.ClearAnonymousIdentifier();
        }
    }

    public class SlugRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string idslug = (string)values[parameterName];
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

            var slugValues = data.Values.Where(v => v.Key.EndsWith("slug")).ToList();

            foreach (var keyValuePair in slugValues)
            {
                string value = (string)keyValuePair.Value;
                string id = value.Split('-')[0];
                string argName = keyValuePair.Key.Substring(0, keyValuePair.Key.Length - "slug".Length);
                data.Values.Add(argName, id);
            }

            return data;
        }
    }

    public static class RouteCollectionExtensionHelper
    {
        public static Route MapSlugRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, object dataTokens = null)
        {
            var route = new SlugRoute(url,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(constraints),
                new RouteValueDictionary(dataTokens),
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