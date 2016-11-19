using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using Microsoft.Owin.Security;
using MyProject;
using System.Linq;
using System.Web;

namespace VIP_Parking.Middleware
{
    public class WindowsAuthenticationService
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

        public WindowsAuthenticationService(IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }

        //Sign in handler
        public AuthenticationResult SignIn(String username, String password)
        {
            // authenticates against your local machine 
            ContextType authenticationType = ContextType.Machine;
            PrincipalContext principalContext = new PrincipalContext(authenticationType);

            bool isAuthenticated = false;
            bool isAdmin = false;
            
            UserPrincipal userPrincipal = null;
            try
            {
                isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
                if (isAuthenticated)                
                    userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);
            }
            catch (Exception e)
            {
                return new AuthenticationResult("Username or Password is not correct");
            }

            //Login information was invalid
            if (!isAuthenticated || userPrincipal == null)
                return new AuthenticationResult("Username or Password is not correct");
                       
            //Probably exraneous code, but may be useful for future developers
            if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
            {
                // here can be a security related discussion whether it is worth 
                // revealing this information
                return new AuthenticationResult("Your account is disabled");
            }


            authenticationManager.SignOut(MyAuthentication.ApplicationCookie);
           
            //Store user's information in session variable
            DirectoryEntry directoryEntry = (userPrincipal.GetUnderlyingObject() as DirectoryEntry);
           
            //Create the user identity
            var identity = CreateIdentity(userPrincipal, directoryEntry);
            
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
            HttpContext sess = HttpContext.Current;

            sess.Session["username"] = "tuser";
            sess.Session["firstname"] = "Test";
            sess.Session["lastname"] = "User";
            sess.Session["email"] = "jkirby1325@gmail.com";
            sess.Session["user_department"] = "IT";
            sess.Session["is_admin"] = false;
            return new AuthenticationResult();
        }

       //Create User Identity. Assigns attributes to the User identity object
        private ClaimsIdentity CreateIdentity(UserPrincipal userPrincipal, DirectoryEntry directoryEntry)
        {
            var identity = new ClaimsIdentity(MyAuthentication.ApplicationCookie, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Active Directory"));
            identity.AddClaim(new Claim(ClaimTypes.Name, userPrincipal.SamAccountName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userPrincipal.SamAccountName));
            if (!String.IsNullOrEmpty(userPrincipal.EmailAddress))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, userPrincipal.EmailAddress));
            }


            // add your own claims if you need to add more information stored on the cookie

            return identity;
        }

        private string GetProperty(DirectoryEntry directoryEntry, string propertyName)
        {
            if (directoryEntry.Properties.Contains(propertyName))
            {
                return directoryEntry.Properties[propertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}