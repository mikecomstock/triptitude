using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Models;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class UserTripsController : TriptitudeController
    {
        private Repo<UserTrip> repo;
        public UserTripsController()
        {
            repo = new Repo<UserTrip>();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int trip_id, string fName, string lName)
        {
            var currentUserTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == trip_id);
            if (currentUserTrip == null) return Redirect("/");
            var trip = currentUserTrip.Trip;

            User newUser = new User
                               {
                                   FirstName = fName,
                                   LastName = lName
                               };
            UserTrip newUserTrip = new UserTrip
                                       {
                                           User = newUser,
                                           IsCreator = false,
                                           Visibility = (byte)UserTrip.UserTripVisibility.Private,
                                           Status = (byte)UserTripStatus.Attending,
                                           StatusUpdatedOnUTC = DateTime.UtcNow
                                       };
            trip.UserTrips.Add(newUserTrip);

            repo.Save();

            return Redirect(Url.Who(trip));
        }

        public ActionResult Delete(int id)
        {
            var userTripToDelete = repo.Find(id);
            var trip = userTripToDelete.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");
            
            if (userTripToDelete.IsCreator)
            {
                Response.StatusCode = 403; // forbidden, does that make sense here??
            }
            else
            {
                repo.Delete(userTripToDelete);
                repo.Save();
            }

            return Json(userTripToDelete.Json(CurrentUser));
        }
    }
}