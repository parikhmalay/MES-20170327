using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Business.Common;

namespace MES.Scheduler
{
    public static class SchedulerExtensions
    {
        /// <summary>
        /// Resolves the Repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="businessBase"></param>
        /// <returns></returns>
        public static T Resolve<T>(this object controller, T businessBase)
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
        public static T Resolve<T>(this object controller, T businessBase, string name)
        {
            return Container.Resolve<T>(businessBase, name);
        }

        public static T Resolve<T>(this object businessObject)
        {
            return Container.Resolve<T>(default(T));
        }
    }
}
