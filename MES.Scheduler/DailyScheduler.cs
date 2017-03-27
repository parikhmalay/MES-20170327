using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using MES.Business.Repositories.Common;
using NPE.Core;
using NPE.Core.Extensions;
using Ninject;
using MES.Scheduler;
namespace MES.Scheduler
{
    public class DailyScheduler : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                JobKey key = context.JobDetail.Key;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                ICommonWorkerRoleRepository repo = new MES.Business.Library.BO.Common.CommonWorkerRole();
                repo.ProcessDaily();
            }
            catch (Exception ex)
            {
                NPE.Business.Common.Container.Resolve<NPE.Core.ILogger>(null).For<DailyScheduler>().Error(String.Format("Exception in  scheduler: <br /><br /> {0}", ex.ToString()), new NPE.Core.Extended.LogMetaData());
            }
        }
    }
}
