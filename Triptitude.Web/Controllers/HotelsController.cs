﻿using System.Linq;
using System.Web.Mvc;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Web.Controllers
{
    public class HotelsController : Controller
    {
        public ActionResult Index(string name)
        {
            IQueryable<ExpediaHotel> hotels = new ExpediaHotelsRepo().Search(name).Take(15);
            ViewBag.Hotels = hotels;
            return View();
        }

        public ActionResult Details(int id)
        {
            ExpediaHotelsRepo expediaHotelsRepo = new ExpediaHotelsRepo();
            ExpediaHotel hotel = expediaHotelsRepo.FindByBaseItemId(id);
            ViewBag.Hotel = hotel;
            return View();
        }
    }
}