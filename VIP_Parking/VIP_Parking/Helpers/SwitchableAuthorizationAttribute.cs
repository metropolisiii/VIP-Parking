using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VIP_Parking.Helpers
{
    public class SwitchableAuthorizationAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool disableAuthentication = false;

#if DEBUG
            disableAuthentication = true;
#endif

            if (disableAuthentication)
                return true;

            return base.AuthorizeCore(httpContext);
        }
    }
}