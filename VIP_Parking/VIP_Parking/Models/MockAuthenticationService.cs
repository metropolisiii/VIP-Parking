using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIP_Parking.Models
{
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Claims;
    using Microsoft.Owin.Security;
    using MyProject;
    using System.Linq;
    using System.Web;

    namespace ActiveDirectoryAuthentication.Models
    {
        public class MockAuthenticationService
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

            public MockAuthenticationService(IAuthenticationManager authenticationManager)
            {
                this.authenticationManager = authenticationManager;
            }


            public AuthenticationResult SignIn(String username, String password)
            {

            // authenticates against your local machine - for development time
            ContextType authenticationType = ContextType.Machine;
            PrincipalContext principalContext = new PrincipalContext(authenticationType);


                bool isAuthenticated = false;
                bool isAdmin = false;
                UserPrincipal userPrincipal = null;
                try
                {
                    isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
                    if (isAuthenticated)
                    {
                        userPrincipal = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, username);
                    }
                }
                catch (Exception)
                {
                    isAuthenticated = false;
                    userPrincipal = null;
                }

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
                if (!IsUserGroupMember(principalContext, userPrincipal, username, "cl-employees"))
                {
                    return new AuthenticationResult("You are not authorized to access this application.");
                }
                if (IsUserGroupMember(principalContext, userPrincipal, username, "cl-members"))
                {
                    isAdmin = true;
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
                sess.Session["is_admin"] = isAdmin;
                return new AuthenticationResult();
            }

            public void addUserToDB(UserPrincipal userPrincipal)
            {

                //Add or insert department in Departments table

                //Add of insert user in Requester table
            }
            public static bool IsUserGroupMember(PrincipalContext context, UserPrincipal user, string userName, string groupName)
            {
                using (PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups())
                {
                    return groups.OfType<GroupPrincipal>().Any(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                }
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
}