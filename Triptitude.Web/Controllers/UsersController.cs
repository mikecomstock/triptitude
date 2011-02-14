using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class UsersController : Controller
    {
        public ActionResult SetDefaultTrip(User currentUser, int id)
        {
            Trip trip = new TripsRepo().Find(id);
            new UsersRepo().SetDefaultTrip(currentUser, trip);
            return Redirect(Url.TripDetails(trip));
        }
    }
}