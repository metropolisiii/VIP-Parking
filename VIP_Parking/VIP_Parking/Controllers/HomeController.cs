using System.Web.Mvc;

namespace VIP_Parking.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "";
            return RedirectToAction("Index","Reservations");
        }
    }
}
