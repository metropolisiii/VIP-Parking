using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using Microsoft.Owin.Security;
using MyProject;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VIP_Parking.Models.Database;
using VIP_Parking.Helpers;

namespace VIP_Parking.Middeware
{
    public class ActiveDirectoryAuthenticationService
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

        public ActiveDirectoryAuthenticationService(IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }
        
        //Sign in handler
        public AuthenticationResult SignIn(String username, String password)
        {
            if (RequestersHelper.IsLocked(username))
                return new AuthenticationResult("Account is locked.");
            
            // authenticates against your Domain AD
            ContextType authenticationType = ContextType.Domain;
            PrincipalContext principalContext = new PrincipalContext(authenticationType, WebConfigurationManager.AppSettings["ActiveDirectoryDomain"], WebConfigurationManager.AppSettings["ActiveDirectoryBaseOU"], username, password);

            bool isAuthenticated = false;
            UserPrincipal userPrincipal = null;
            
            //Attempt the authentication
            try
            {
                isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
                //To do - Looking into FindByIdentity initial exception catch
                if (isAuthenticated)
                    userPrincipal = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, username);
            }
            catch (Exception)
            {
                isAuthenticated = false;
                userPrincipal = null;
            }

            //Login information is invalid
            if (!isAuthenticated || userPrincipal == null)
            {
                return new AuthenticationResult("Username or Password is not correct");
            }

            if (userPrincipal.IsAccountLockedOut())
            {
                // here can be a security related discussion weather it is worth 
                // revealing this information
                return new AuthenticationResult("Your account is locked.");
            }

            if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
            {
                // here can be a security related discussion weather it is worth 
                // revealing this information
                return new AuthenticationResult("Your account is disabled");
            }          
            var identity = CreateIdentity(userPrincipal);

            authenticationManager.SignOut(MyAuthentication.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);

            //Store user's information in session variable
            DirectoryEntry directoryEntry = (userPrincipal.GetUnderlyingObject() as DirectoryEntry);
            HttpContext sess = HttpContext.Current;

            sess.Session["username"] = username;
            sess.Session["firstname"] = userPrincipal.GivenName;
            sess.Session["lastname"] = userPrincipal.Surname;
            sess.Session["email"] = userPrincipal.EmailAddress;
            sess.Session["user_department"] = GetProperty(directoryEntry, "Department");
            sess.Session["is_admin"] = false;

            return new AuthenticationResult();
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
        private ClaimsIdentity CreateIdentity(UserPrincipal userPrincipal)
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
    }
}