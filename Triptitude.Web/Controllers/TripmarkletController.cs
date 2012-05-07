using System.Web.Mvc;
using Triptitude.Biz;
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
            ViewBag.ParsedTitle = title.Trim();

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
                var repo = new TripsRepo();
                Trip trip = repo.Save(form, CurrentUser);
                CurrentUser.DefaultTrip = trip;
                trip.AddHistory(CurrentUser, HistoryAction.CreatedTrip);
                repo.Save();

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