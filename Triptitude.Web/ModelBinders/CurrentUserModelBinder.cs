using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.ModelBinders
{
    public class CurrentUserModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.HttpContext.Request.IsAuthenticated)
            {
                int userId = int.Parse(controllerContext.HttpContext.User.Identity.Name.Split('|')[0]);
                User user = new UsersRepo().Find(userId);
                return user;
            }

            return null;
        }
    }
}