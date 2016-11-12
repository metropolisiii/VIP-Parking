using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using ActiveDirectoryAuthentication.Models;
using Microsoft.Owin.Security;
using MyProject;
using VIP_Parking.Models.Database;
using System.Linq;

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
                int deptID = 0;

                //If the department isn't in the Department table, insert it
                VIPPARKINGEntities1 db = new VIPPARKINGEntities1();

                string user_department = Session["user_department"].ToString();
                var d = db.Departments.Where(i => i.Dept_name == user_department);
                if (d.Count() == 0)
                {
                    var department = new VIP_Parking.Models.Database.Department
                    {
                        Dept_name = (string)Session["user_department"]
                    };
                    db.Departments.Add(department);
                    db.SaveChanges();
                    deptID = department.Dept_ID;
                }
                else
                {
                    foreach(var rec in d)
                    {
                        deptID = rec.Dept_ID;
                    }                   
                }

                //Insert or update Requestor table
                string username = Session["username"].ToString();
                var requester_results = db.Requesters.Where(u => u.Username == username);
                if (requester_results.Count() == 0)
                {
                    var requester = new VIP_Parking.Models.Database.Requester
                    {
                        Username = (string)Session["username"],
                        Firstname = (string)Session["firstname"],
                        Lastname = (string)Session["lastname"],
                        Dept_ID = deptID,
                        Email = (string)Session["email"]
                    };
                    db.Requesters.Add(requester);
                    db.SaveChanges();
                    Session["userID"] = requester.Requester_ID;
                }
                else
                {
                    foreach (var requester in requester_results)
                    {
                        requester.Firstname = (string)Session["firstname"];
                        requester.Lastname = (string)Session["lastname"];
                        requester.Dept_ID = deptID;
                        requester.Email = (string)Session["email"];

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        Session["userID"] = requester.Requester_ID;
                    }

                }

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