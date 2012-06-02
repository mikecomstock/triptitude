using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Models;
using Triptitude.Biz.Services;
using Triptitude.Biz.Extensions;

namespace Triptitude.Web.Controllers
{
    public class CronController : Controller
    {
        private static readonly byte[] AlertableActions = new[] {
            (byte)HistoryAction.CreateActivity,
            (byte)HistoryAction.UpdateActivity,
            (byte)HistoryAction.DeletedActivity,
            (byte)HistoryAction.AddUserToTrip,
            (byte)HistoryAction.CreatedNote
        };

        public ActionResult Index(string pass)
        {
            if (pass != "reijfdIJFDKjewidjsKDJ") return Redirect("/");

            StringBuilder sb = new StringBuilder();
            var repo = new Repo<UserTrip>();

            var halfHourAgo = DateTime.UtcNow.AddMinutes(-30);
            //var halfHourAgo = DateTime.UtcNow.AddSeconds(-30);
            var userTrips = repo.FindAll().Where(ut =>
                ut.UpToDateAsOfUTC < ut.Trip.ModifiedUTC // changed since we last sent them an email
                && ut.Trip.ModifiedUTC < halfHourAgo // exclude trips that are getting changed RIGHT NOW (or somewhat recently)
                && ut.User.EmailWhenTripsUpdated // make sure they actually WANT to get an email
                && !ut.Deleted // don't include deleted trips!
            ).ToList();

            foreach (var userTrip in userTrips)
            {
                var validHistories = userTrip.Trip.Histories.Where(h =>
                    h.CreatedOnUTC > userTrip.UpToDateAsOfUTC // only alert them on histories that have happend since we last updated them
                    && h.User != userTrip.User // they already know what THEY have done, so don't alert them on those things
                    && AlertableActions.Contains(h.Action) // only alert on certain actions
                ).OrderBy(h => h.Id);

                if (!validHistories.Any()) continue;

                string subject;
                var updaters = validHistories.Select(h => h.User).Distinct();
                var firstUpdater = updaters.First();
                var numUpdaters = updaters.Count();
                if (numUpdaters == 1) { subject = string.Format("{0} has updated your trip", firstUpdater.NameOrAnon); }
                else if (numUpdaters == 2)
                {
                    var updaterNames = String.Join(" and ", updaters.Select(u => u.NameOrAnon));
                    subject = string.Format("{0} have updated your trip", updaterNames);
                }
                else { subject = string.Format("{0} and {1} others have updated your trip", firstUpdater.NameOrAnon, numUpdaters - 1); }

                StringBuilder body = new StringBuilder();
                body.AppendFormat("<h2><a href=\"{1}\">{0}</a></h2>", userTrip.Trip.Name, Url.Details(userTrip.Trip, true));

                body.Append("<ul>");
                var historyDescriptions = validHistories.Select(h => h.ToString(userTrip.User, false));
                historyDescriptions.ToList().ForEach(hd => body.AppendFormat("<li>{0}</li>", hd));
                body.Append("</ul>");

                body.AppendFormat("<a href=\"{0}\">View Full Trip</a>", Url.Details(userTrip.Trip, true));

#if DEBUG
                sb.AppendFormat("<div> <p>To: {0}</p> <p>Subject: {1}</p> <p>Body: {2}</p> </div>", userTrip.User.Email, subject, body);
#endif

                // actually send the email!
                userTrip.UpToDateAsOfUTC = DateTime.UtcNow;
                repo.Save();
                var sent = EmailService.SendTripUpdate(userTrip.User, subject, body.ToString());

                if (!sent)
                {
                    var b = string.Format("To User: {0} Email: {1}, Body: {2}", userTrip.User.Id, userTrip.User.Email, body);
                    EmailService.SendAdmin("Error sending email update!", b);
                }

            }

            sb.Append("<p>Success</p>");
            return Content(sb.ToString());
        }
    }
}