using System.Web.Mvc;

namespace Triptitude.Web.Areas.API
{
    public class APIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "API";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("API_default", "API/V1/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
