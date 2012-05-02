using System;
using System.Configuration;
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
            routes.MapRoute("Tripmarklet", "tripmarklet", new { controller = "home", action = "tripmarklet" });
            routes.MapRoute("TripmarkletJS", "tripmarklet.js", new { controller = "home", action = "tripmarkletJS" });


            //routes.MapRoute("TRIP WHO POST", "trips/{trip_id}/who", new { controller = "trips", action = "WhoCreate" }, new { httpMethod = new HttpMethodConstraint("POST") });
            ////todo: figure out method for this:
            //routes.MapRoute("TRIP WHO DELETE", "trips/{trip_id}/whodelete", new { controller = "trips", action = "Whodelete" }, new { httpMethod = new HttpMethodConstraint("POST") });


            routes.MapRoute("REST PUT", "{controller}/{id}", new { action = "Update" }, new { httpMethod = new HttpMethodConstraint("PUT") });
            routes.MapRoute("REST DELETE", "{controller}/{id}", new { action = "Delete" }, new { httpMethod = new HttpMethodConstraint("DELETE") });
            routes.MapRoute("REST POST", "{controller}", new { action = "Create" }, new { httpMethod = new HttpMethodConstraint("POST") });

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
            js.AddFile("~/Scripts/backbone.js");
            js.AddFile("~/Scripts/jquery.placeholder.js");
            js.AddFile("~/Scripts/triptitude.js");
            js.AddFile("~/Scripts/app/TT.js");
            js.AddDirectory("~/Scripts/app/models", "*.js", true);
            js.AddDirectory("~/Scripts/app/views", "*.js", true);

            BundleTable.Bundles.Add(js);

            /* Trip Editor */
            Bundle editor = new Bundle("~/Content/editor", new CssMinify());
            editor.AddFile("~/Content/Editor.css");
            editor.AddFile("~/Content/themes/base/jquery.ui.core.css");
            editor.AddFile("~/Content/themes/base/jquery.ui.datepicker.css");
            editor.AddFile("~/Content/themes/base/jquery.ui.theme.css");
            BundleTable.Bundles.Add(editor);

            /* Tripmarklet (with string replace) */
            Bundle tripmarklet = new Bundle("~/Scripts/tripmarklet.js", new JsReplaceMinify());
            tripmarklet.AddFile("~/Scripts/tripmarklet_template.js");
            BundleTable.Bundles.Add(tripmarklet);

            /* Editor (used by tripmarklet) */
            Bundle editorJs = new Bundle("~/Scripts/editor", new JsMinify());
            editorJs.AddFile("~/Scripts/jquery-1.7.1.min.js");
            editorJs.AddFile("~/Scripts/jquery-ui-1.8.18.min.js");
            editorJs.AddFile("~/Scripts/underscore.js");
            editorJs.AddFile("~/Scripts/backbone.js");
            editorJs.AddFile("~/Scripts/app/TT.js");
            editorJs.AddDirectory("~/Scripts/app/models", "*.js", true);
            editorJs.AddDirectory("~/Scripts/app/views", "*.js", true);
            BundleTable.Bundles.Add(editorJs);
        }

        public void Profile_OnMigrateAnonymous(object sender, ProfileMigrateEventArgs args)
        {
            int userId = int.Parse(args.Context.User.Identity.Name.Split('|')[0]);
            new UsersRepo().MigrateAnonymousUser(args.AnonymousID, userId);
            AnonymousIdentificationModule.ClearAnonymousIdentifier();
        }
    }

    public class JsReplaceMinify : JsMinify
    {
        public override void Process(BundleContext context, BundleResponse response)
        {
            string siteRoot = ConfigurationManager.AppSettings["RootUrl"];
            response.Content = response.Content.Replace("{siteRoot}", siteRoot);
            base.Process(context, response);
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