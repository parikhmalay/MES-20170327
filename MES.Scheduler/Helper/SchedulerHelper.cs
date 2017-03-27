using NPE.Business.Common;
using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MES.Business.Library.Extensions;
using Ninject.Extensions.Xml;
using Ninject;
using MES.SchedulerService.Extensions;
namespace MES.SchedulerService.Common
{
    public class SchedulerHelper : AbstractSchedulerHelper
    {
        public SchedulerHelper()
            : base(new string[]
                {
                    Environment.CurrentDirectory + @"\MES.Business.Mapping.dll",
                    Environment.CurrentDirectory + @"\MES.ModelBuilder.Library.dll"
                })
        { }

        public override void InitKernel()
        {
            //One Time Initialization

            var kernel = new GeneralKernel(); //Load Kernel 
            kernel.DirtyLoadRepos(
                Environment.CurrentDirectory + @"\WebClient.References.xml");
        }



    }
}
