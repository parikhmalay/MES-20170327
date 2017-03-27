using NPE.Web.Common.Constants;
using NPE.Web.Common.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MES.API.Attributes
{
    public class CheckForAPIKeyAttribute : System.Web.Mvc.ActionFilterAttribute //BaseActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext actionContext)
        {
            string apiKey = null;
            var request = actionContext.RequestContext.HttpContext.Request;

            apiKey = request.Headers.Get(HeaderConstants.APIKeyHeader);
            if (apiKey == null)
                apiKey = request.QueryString[HeaderConstants.APIKeyHeader];

            var valid = (apiKey != null);
            if (valid)
            {
                //validate the APIKEY aginst the Client.
                if (actionContext.Controller is DefaultWebControllerBase)
                {
                    //TODO: Check if the ApiKey is already there in OWIN Claims and allow as per that. Dont fetch from DB.

                    //validate api key aginst the client id.
                    valid = (actionContext.Controller as DefaultWebControllerBase).ValidateClient(apiKey);
                }
                else
                    valid = false;
            }

            //if not valid.
            if (!valid)
                throw new Exception("API Key not found.");

            base.OnActionExecuting(actionContext);
        }
    }
}