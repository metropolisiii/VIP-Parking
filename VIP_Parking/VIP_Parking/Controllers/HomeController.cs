using System.Web.Mvc;

namespace VIP_Parking.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "";
            if (Session["userID"] != null)
                return RedirectToAction("Index","Reservations");
            else
                return RedirectToAction("Logoff", "Login");
        }
    }
}
