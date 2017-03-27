using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MES
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception exception = Server.GetLastError();
                //Response.Clear();
                HttpException httpException = exception as HttpException;
                if (httpException != null)
                {
                    string action;
                    switch (httpException.GetHttpCode())
                    {
                        case 404:
                            action = "NotFound";
                            break;
                        case 500:
                            action = "InternalError";
                            break;
                        default:
                            action = "CommonError";
                            break;
                    }
                    if (!string.IsNullOrEmpty(action))
                    {
                        Server.ClearError();
                        Response.Redirect(String.Format("~/Error/{0}", action));
                    }
                }
                Server.ClearError();
                Response.Redirect("~/Error/CommonError");
            }
            catch (System.Threading.ThreadAbortException ex1)
            {

            }
            catch (System.Web.HttpException ex2)
            {

            }

        }

    }
}
