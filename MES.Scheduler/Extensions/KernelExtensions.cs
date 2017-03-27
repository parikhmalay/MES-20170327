using MES.Business.Library;
using MES.Business.Library.BO.Identity;
using Ninject;
using NPE.Business.Common;
using NPE.Core;
using NPE.Web.Common.Logging;

namespace MES.SchedulerService.Extensions
{
    public static class KernelExtensions
    {
        //private static bool KernelReposLoaded = false;

        /// <summary>
        /// Loads all Repositories in Default fashion
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="repoDir"></param>
        public static void LoadRepos(this GeneralKernel kernel, string repoDir, bool bindContextInRequest = true, bool initWebKernel = false, bool initTestKernel = false)
        {
            Container.
            GetInstance(kernel). //Kernel Object
                LoadRepos(repoDir, //Path of XML file containing list of all repos
                Constants.Contexts, bindContextInRequest). //Context2
                //AddRepo(typeof(IClientValidation), typeof(ClientValidation), true). //validation repository to load client info
                 AddSingletonRepo(typeof(ILogger), typeof(BusinessLogger)); //Logger

            if (initWebKernel && Container.WebKernel == null)
                Container.WebKernel = kernel;

            if (initTestKernel && Container.TestKernel == null)
                Container.TestKernel = kernel;
        }

        /// <summary>
        /// Loads Repositories by Manipulating XML Configuration File such that Scope in the XML will be changed to - Transient
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="repoDir">The repo dir.</param>
        /// <param name="bindContextInRequest">if set to <c>true</c> [bind context in request].</param>
        /// <param name="initWebKernel">if set to <c>true</c> [initialize web kernel].</param>
        /// <param name="initTestKernel">if set to <c>true</c> [initialize test kernel].</param>
        public static void DirtyLoadRepos(this GeneralKernel kernel, string repoDir)
        {
            Container.
            GetInstance(kernel). //Kernel Object
                CustomLoadRepos(repoDir, //Custom Loading of Repositories with Dirty Fix.
                Constants.Contexts, false). //Context2
                //AddRepo(
                    //typeof(IClientValidation), //Default Client Validation Repository
                    //typeof(ClientValidation), true).
                    AddSingletonRepo(typeof(ILogger), typeof(BusinessLogger));

            if (Container.TestKernel == null)
                Container.TestKernel = kernel;
        }
        
    }
}
