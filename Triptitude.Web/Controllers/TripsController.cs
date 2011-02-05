using System.Web.Mvc;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Triptitude.Web.Helpers;

namespace Triptitude.Web.Controllers
{
    public class TripsController : Controller
    {
        //
        // GET: /Trips/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Trips/Details/5

        public ActionResult Details(int id)
        {
            ViewBag.Trip = new TripsRepo().Find(id);
            return View();
        }

        //
        // GET: /Trips/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Trips/Create

        [HttpPost]
        public ActionResult Create(TripCreate form)
        {
            try
            {
                Trip trip = new TripsRepo().Save(form);
                return Redirect(Url.TripDetails(trip));
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Trips/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Trips/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Trips/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Trips/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
