using System;
using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Biz.Services;

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

            var emailInvite = userTrip.EmailInvites.OrderByDescending(ei => ei.Id).FirstOrDefault();

            ViewBag.UserTrip = userTrip;
            ViewBag.EmailInvite = emailInvite;
            ViewBag.EmailTaken = new UsersRepo().FindByEmail(emailInvite.Email) != null;

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

        [HttpPost]
        public ActionResult Accept(Guid guid)
        {
            var userTrip = repo.FindAll().FirstOrDefault(ut => ut.Guid == guid);
            CurrentUser.DefaultTrip = userTrip.Trip;

            // Only change the userTrip's user if the currentUser doesn't already own the trip
            if (!CurrentUser.OwnsTrips(userTrip.Trip))
            {
                userTrip.User = CurrentUser;
                userTrip.InviteAccepted = true;
                userTrip.UpToDateAsOfUTC = DateTime.UtcNow;
                //TODO: delete the orphaned user or not???
                userTrip.Trip.AddHistory(CurrentUser, HistoryAction.AcceptInvitation);
            }

            repo.Save();
            return Redirect(Url.Details(userTrip.Trip));
        }
    }
}