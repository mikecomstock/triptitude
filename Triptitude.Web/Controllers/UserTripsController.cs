using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;
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

            // If they already accepted the invitation and they own the trip, just redirect them
            if (CurrentUser.OwnsTrips(userTrip.Trip))
                Redirect(Url.Details(userTrip.Trip));

            ViewBag.UserTrip = userTrip;
            return View();
        }

        public ActionResult Create(int usertrip_id, string email)
        {
            var userTrip = repo.Find(usertrip_id);
            var trip = userTrip.Trip;
            if (!CurrentUser.OwnsTrips(trip)) return Redirect("/");

            EmailInvite emailInvite = new EmailInvite
            {
                UserTrip = userTrip,
                Email = email,
                Created_On = DateTime.UtcNow
            };
            userTrip.EmailInvites.Add(emailInvite);

            EmailService.SentEmailInvite(emailInvite, Url);
            trip.AddHistory(CurrentUser, HistoryAction.CreateInvitation, userTrip.User.Id);

            repo.Save();

            return Json(userTrip.Json(CurrentUser, Url));
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
        //[ValidateAntiForgeryToken]
        public ActionResult Create(int trip_id, string firstName, string lastName, bool you = false)
        {
            var currentUserTrip = CurrentUser.UserTrips.SingleOrDefault(ut => ut.Trip.Id == trip_id);
            if (currentUserTrip == null) return Redirect("/");
            var trip = currentUserTrip.Trip;

            if (you)
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
            }

            return Json(jsonToReturn);
        }
    }
}