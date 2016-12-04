using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using Microsoft.Owin.Security;
using MyProject;
using System.Linq;
using System.Web;
using VIP_Parking.Models.Database;
using System.Web.Security;
using VIP_Parking.Helpers;

namespace VIP_Parking.Middleware
{
    public class DatabaseAuthenticationService
    {
        public class AuthenticationResult
        {
            public AuthenticationResult(string errorMessage = null)
            {
                ErrorMessage = errorMessage;
            }

            public String ErrorMessage { get; private set; }
            public Boolean IsSuccess => String.IsNullOrEmpty(ErrorMessage);
        }

        private readonly IAuthenticationManager authenticationManager;

        public DatabaseAuthenticationService(IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }

        //Sign in handler
        public AuthenticationResult SignIn(String username, String password)
        {
            using (VIPPARKINGEntities1 db = new VIPPARKINGEntities1())
            {

                Requester requester = db.Requesters.Where(user => user.Username == username && user.Password == password).SingleOrDefault();
                if (RequestersHelper.IsLocked(username))
                    return new AuthenticationResult("Account is locked.");

                // User found in the database
                if (requester != null)
                {
                    FormsAuthentication.SetAuthCookie(username, false);
                }
                else
                {
                    return new AuthenticationResult("Username or Password is not correct");
                }
                HttpContext sess = HttpContext.Current;

                sess.Session["username"] = requester.Username;
                sess.Session["firstname"] = requester.Firstname;
                sess.Session["lastname"] = requester.Lastname;
                sess.Session["email"] = requester.Email;
                sess.Session["user_department"] = requester.Department.Dept_name;
                sess.Session["isAdmin"] = requester.IsAdmin;
                sess.Session["deptID"] = DepartmentsHelper.Upsert((string)sess.Session["user_department"]);

                //Insert or update Requestor table
                sess.Session["userID"] = RequestersHelper.Upsert(requester.Username, requester.Firstname, requester.Lastname, requester.Email, (int)sess.Session["deptID"]);
            }

            return new AuthenticationResult();
        }
    }
}