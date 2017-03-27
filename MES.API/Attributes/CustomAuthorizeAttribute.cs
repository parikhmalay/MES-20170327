using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MES.API.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAttributePosition
    {
        public int Position
        {
            get;set;
        }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //For some unknown reasons Context principal is not carried forward to thread principle 
            //so explicitly assignining with below condition
            if (!(System.Threading.Thread.CurrentPrincipal).GetType().Equals(typeof(System.Security.Claims.ClaimsPrincipal)))
                System.Threading.Thread.CurrentPrincipal = HttpContext.Current.User;

            return base.IsAuthorized(actionContext);
        }
    }
}