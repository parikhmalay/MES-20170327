using Ninject;
using NPE.Business.Common;
using NPE.Web.Common.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MES.API.Extensions
{
    public static class ApiControllerExtensions
    {
        /// <summary>
        /// Resolves the Repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="businessBase"></param>
        /// <returns></returns>
        public static T Resolve<T>(this ApiController controller, T businessBase)
        {
            return Container.Resolve<T>(businessBase);
        }

        /// <summary>
        /// Resolves the specified business base.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">The controller.</param>
        /// <param name="businessBase">The business base.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T Resolve<T>(this ApiController controller, T businessBase, string name)
        {
            return Container.Resolve<T>(businessBase, name);
        }
        
    }
}