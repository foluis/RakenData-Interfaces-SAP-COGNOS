using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RankenData.InterfacesSAPCognos.Web
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {         
            context.Result = new RedirectResult("/UnauthorizedRequest.html");
        }
    }
}