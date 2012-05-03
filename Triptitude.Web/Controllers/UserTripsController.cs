using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Models;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class InvitationsController : TriptitudeController
    {
        private Repo<UserTrip> repo;
        public InvitationsController()
        {
            repo = new Repo<UserTrip>();
        }

        public ActionResult Details(Guid guid)
        {
            var userTrip = repo.FindAll().FirstOrDefault(ut => ut.Guid == guid);
            ViewBag.UserTrip = userTrip;

            return View();
        }

        public ActionResult Update(Guid guid)
        {
            var userTrip = repo.FindAll().FirstOrDefault(ut => ut.Guid == guid);

            if (string.IsNullOrWhiteSpace(CurrentUser.FirstName)) CurrentUser.FirstName = userTrip.User.FirstName;
            if (string.IsNullOrWhiteSpace(CurrentUser.LastName)) CurrentUser.LastName = userTrip.User.LastName;
            userTrip.User = CurrentUser;
            CurrentUser.DefaultTrip = userTrip.Trip;
            //TODO: delete the orphaned user or not???
            repo.Save();
            return Redirect(Url.Details(userTrip.Trip));
        }
    }

    public class UserTripsController : TriptitudeController
    {
        private Repo<UserTrip> repo;
        public UserTripsController()
        {
            repo = new Repo<UserTrip>();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int trip_id, string fName, string lName, bool you)
        {
            var currentUserTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == trip_id);
            if (currentUserTrip == null) return Redirect("/");
            var trip = currentUserTrip.Trip;

            if (you)
            {
                CurrentUser.FirstName = fName;
                CurrentUser.LastName = lName;
            }
            else
            {
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
                    Created_On = DateTime.UtcNow,
                    Guid = Guid.NewGuid()
                };
                trip.UserTrips.Add(newUserTrip);
            }

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