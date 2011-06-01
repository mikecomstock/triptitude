using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.ModelBinders
{
    public class CurrentUserModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var usersRepo = new UsersRepo();
            User user;

            if (controllerContext.HttpContext.Request.IsAuthenticated)
            {
                int userId = int.Parse(controllerContext.HttpContext.User.Identity.Name.Split('|')[0]);
                user = usersRepo.Find(userId);
            }
            else
            {
                user = usersRepo.FindOrInitialize(controllerContext.HttpContext.Request.AnonymousID);
            }

            return user;
        }
    }
}