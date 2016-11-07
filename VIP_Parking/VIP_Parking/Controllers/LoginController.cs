using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ActiveDirectoryAuthentication.Models;
using Microsoft.Owin.Security;
using MyProject;

namespace ActiveDirectoryAuthentication.Controllers
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
        public virtual ActionResult Index(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // usually this will be injected via DI. but creating this manually now for brevity
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            var authService = new AdAuthenticationService(authenticationManager);

            var authenticationResult = authService.SignIn(model.Username, model.Password);

            if (authenticationResult.IsSuccess)
            {
                // we are in!
                //Insert or update Requestor table
                var user = User.Identity.Name;
               
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", authenticationResult.ErrorMessage);
            return View(model);
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        public virtual ActionResult Logoff()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(MyAuthentication.ApplicationCookie);

            return RedirectToAction("Index");
        }
    }


    public class LoginViewModel
    {
        [Required, AllowHtml]
        public string Username { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}