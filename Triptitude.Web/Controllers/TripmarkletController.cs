using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class TripmarkletController : TriptitudeController
    {
        public ActionResult Tripmarklet(string url, string title)
        {
            if (CurrentUser.DefaultTrip == null)
                return Redirect("/tripmarklet/createtrip");
            
            ViewBag.URL = url;
            ViewBag.ParsedTitle = title;

            var currentUserData = CurrentUser.Json(CurrentUser);
            ViewBag.CurrentUserData = currentUserData;
            return View();
        }

        [HttpGet]
        public ActionResult CreateTrip(string returnURL)
        {
            ViewBag.Form = new NewCreateTripForm();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTrip(NewCreateTripForm form)
        {
            if (ModelState.IsValid)
            {
                Trip trip = new TripsRepo().Save(form, CurrentUser);
                new UsersRepo().SetDefaultTrip(CurrentUser, trip);
                new HistoriesRepo().Create(CurrentUser, trip, HistoryAction.Created, HistoryTable.Trips, trip.Id);
                return Redirect("/tripmarklet/tripmarklet");
            }
            else
            {
                ViewBag.Form = form;
                return View();
            }
        }
    }
}