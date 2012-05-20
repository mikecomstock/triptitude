using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Triptitude.Biz;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Controllers
{
    public class CronController : Controller
    {
        public ActionResult Index(string pass)
        {
            if (pass != "reijfdIJFDKjewidjsKDJ") return Redirect("/");

            StringBuilder sb = new StringBuilder();
            var repo = new Repo<History>();

            // Exclude trips that are currently actively being edited
            var halfHourAgo = DateTime.UtcNow.AddMinutes(-3);
            var volatileTrips = repo.FindAll().Where(h => h.CreatedOnUTC > halfHourAgo).Select(t => t.Trip);
            
            var hourAgo = DateTime.UtcNow.AddHours(-1);
            var nonVolatileHistories = repo.FindAll().Where(h => h.CreatedOnUTC > hourAgo && !volatileTrips.Contains(h.Trip));
            //nonVolatileHistories = nonVolatileHistories.Where(h => h.Action == (byte)HistoryAction.UpdateActivity);

            // 
            var eligibleTrips = nonVolatileHistories.Select(h => h.Trip).Distinct();
            foreach (var eligibleTrip in eligibleTrips)
            {
                var userTrips = eligibleTrip.UserTrips;
                foreach (var userTrip in userTrips)
                {
                    if (string.IsNullOrWhiteSpace(userTrip.User.Email) || !userTrip.User.EmailWhenTripsUpdated)
                        continue;

                    // look for histories that weren't authored by this user
                    var eligibleHistories = nonVolatileHistories.Where(h => h.Trip.Id == eligibleTrip.Id && h.User.Id != userTrip.User.Id).ToList();

                    // if someone other than this user has changed the trip, then alert this user
                    if (eligibleHistories.Any())
                    {
                        sb.AppendFormat("<p>send email to {0} (id: {1}, email: {2}) for trip {3} (id: {4})",
                                        userTrip.User.FullName, userTrip.User.Id, userTrip.User.Email,
                                        userTrip.Trip.Name, userTrip.Trip.Id);

                        // send email
                        var tripChangedBy = eligibleHistories.Select(h => h.User).Distinct();
                        var users = String.Join(",", tripChangedBy.Select(u => u.FullName).ToArray());
                        sb.AppendFormat("<br/>trip was changed by: {0}</p>", users);
                    }

                }
            }

            return Content(sb.ToString());
        }
    }
}