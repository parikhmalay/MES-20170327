using Elmah.Contrib.WebApi;
using NPE.Web.Common;
using NPE.Web.Common.Constants;
using NPE.Web.Common.Routing;
using NPE.Web.Common.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MES.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Web API routes
            config.MapHttpAttributeRoutes();

            MES.API.DefaultWebApiBootstrapper.Initialize(!ConfigReader.GetAppSettingValue<bool>(ConfigKeyConstants.DisableEntireAPI, false), new List<NPE.Core.IRouteInfo>() { new RouteInfo("api", "{controller}/{action}/{id}", "MES.API.Controllers") });

            //ELMAH
            config.Filters.Add(new ElmahHandleErrorApiAttribute());

            //config.Routes.MapHttpRoute(
            //    name: "TestCallApi",
            //    routeTemplate: "api/TestCall/{controller}/{Action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
