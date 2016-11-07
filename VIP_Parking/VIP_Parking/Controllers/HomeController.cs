using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIP_Parking.Models;

namespace VIP_Parking.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "";
            Reservation reservation = new Reservation();
            return View(reservation);
        }
    }
}
