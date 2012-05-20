using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Models;

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
        public ActionResult Create(int trip_id, string firstName, string lastName, bool you = false)
        {
            var currentUserTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == trip_id);
            if (currentUserTrip == null) return Redirect("/");
            var trip = currentUserTrip.Trip;

            if (you && string.IsNullOrWhiteSpace(CurrentUser.FirstName) && string.IsNullOrWhiteSpace(CurrentUser.LastName))
            {
                CurrentUser.FirstName = firstName;
                CurrentUser.LastName = lastName;
                repo.Save();
                return Redirect(Url.Who(trip));
            }
            else
            {
                User newUser = new User
                {
                    FirstName = firstName,
                    LastName = lastName
                };
                UserTrip newUserTrip = new UserTrip
                {
                    User = newUser,
                    IsCreator = false,
                    Visibility = (byte)UserTrip.UserTripVisibility.Private,
                    Created_On = DateTime.UtcNow,
                    UpToDateAsOfUTC = DateTime.UtcNow,
                    Guid = Guid.NewGuid()
                };
                trip.UserTrips.Add(newUserTrip);
                newUser.DefaultTrip = trip;
                repo.Save();

                trip.AddHistory(CurrentUser, HistoryAction.AddUserToTrip, newUserTrip.User.Id);
                repo.Save();

                return Json(newUserTrip.Json(CurrentUser, Url));
            }
        }

        public ActionResult Delete(int id)
        {
            var userTripToDelete = repo.Find(id);
            var trip = userTripToDelete.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            var userTripUser = userTripToDelete.User;
            var jsonToReturn = userTripToDelete.Json(CurrentUser, Url);

            if (userTripToDelete.IsCreator)
            {
                Response.StatusCode = 403; // forbidden, does that make sense here??
            }
            else
            {
                repo.Delete(userTripToDelete);
                trip.AddHistory(CurrentUser, HistoryAction.RemoveUserFromTrip, userTripUser.Id);
                repo.Save();

                userTripUser.DefaultTrip = userTripUser.Trips(userTripUser).OrderBy(t => t.Id).LastOrDefault();
                repo.Save();
            }

            return Json(jsonToReturn);
        }
    }
}