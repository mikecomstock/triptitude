using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class TriptitudeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.CurrentUser = CurrentUser;
            //connection = (SignalR.PersistentConnection)HttpContext.Application["SignalR"];
            base.OnActionExecuting(filterContext);
        }

        private User _CurrentUser;
        public User CurrentUser
        {
            get
            {
                if (_CurrentUser == null)
                {
                    var usersRepo = new UsersRepo();

                    if (HttpContext.Request.IsAuthenticated)
                    {
                        int userId = int.Parse(HttpContext.User.Identity.Name.Split('|')[0]);
                        _CurrentUser = usersRepo.Find(userId);
                    }
                    else
                    {
                        _CurrentUser = usersRepo.FindOrInitialize(HttpContext.Request.AnonymousID);
                    }
                }
                return _CurrentUser;
            }
        }

        //private SignalR.PersistentConnection connection;

        //public void Send(string s)
        //{
        //    connection.Connection.Broadcast(s);
        //}
    }
}