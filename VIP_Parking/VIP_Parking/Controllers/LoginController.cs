using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using MyProject;
using VIP_Parking.ViewModels;
using VIP_Parking.Middleware;
using System.Web.Security;
using VIP_Parking.Helpers;

namespace VIP_Parking.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public virtual ActionResult Index()
        {
            if (User.Identity.IsAuthenticated) //If user is already logged in, redirect them to the home page
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Index(LoginVM model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            #if DEBUG
            var authService = new DatabaseAuthenticationService(authenticationManager);
            #else
            var authService = new ActiveDirectoryAuthenticationService(authenticationManager);
            #endif
            var authenticationResult = authService.SignIn(model.Username, model.Password);

            if (authenticationResult.IsSuccess)
            {
                //Insert or update the requesters department
                Session["deptID"] = DepartmentsHelper.Upsert((string)Session["user_department"]);

                //Insert or update Requestor table
                Session["userID"] = RequestersHelper.Upsert((string)Session["username"], (string)Session["firstname"], (string)Session["lastname"], (string)Session["email"], (int)Session["deptID"]);

                //Is the user an admin
                Session["isAdmin"] = RequestersHelper.IsAdmin((int)Session["userID"]);                

                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", authenticationResult.ErrorMessage);
            return View(model);
        }

        //Redirect back to the page the user was on
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        //Log off
        public virtual ActionResult Logoff()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(MyAuthentication.ApplicationCookie);
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}