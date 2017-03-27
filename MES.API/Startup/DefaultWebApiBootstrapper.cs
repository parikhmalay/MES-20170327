using NPE.Core;
using NPE.Web.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MES.API
{
    public class DefaultWebApiBootstrapper : NPE.Web.Common.Startup.DefaultWebApiBootstrapper
    {
        private DefaultWebApiBootstrapper(bool enableBootstrapper, List<IRouteInfo> routes)
            : base(enableBootstrapper, routes)
        {
        }

        private static bool _IsBootstrapperInited = false;

        public static void Initialize(bool enableBootstrapper, List<IRouteInfo> routes)
        {
            if (!_IsBootstrapperInited)
            {
                new DefaultWebApiBootstrapper(enableBootstrapper, routes);
                _IsBootstrapperInited = true;
            }
        }

        public override void LoadMaps(Type mappingAssemblyType)
        {
            //TODO:Load maps when needed
            if (mappingAssemblyType != null)
                MapLoader.LoadMaps(mappingAssemblyType);
            else
            {
                MapLoader.LoadMaps(ConfigurationManager.AppSettings["Business.Maps"] ?? System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"bin\MES.Business.Mapping.dll");
                MapLoader.LoadMaps(ConfigurationManager.AppSettings["ModelBuilder.Maps"] ?? System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"bin\MES.ModelBuilder.Library.dll");
            }
            
        }
    }
}