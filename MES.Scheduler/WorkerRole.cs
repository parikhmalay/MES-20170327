using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Quartz;
using Quartz.Impl;
using NPE.Core;

namespace MES.Scheduler
{
    public class WorkerRole : RoleEntryPoint
    {
        IScheduler schedDailyNight;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private MES.SchedulerService.Common.SchedulerHelper helper = new MES.SchedulerService.Common.SchedulerHelper();
        public override void Run()
        {
            Trace.TraceInformation("MES.Scheduler is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            catch (Exception ex)
            { }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;
            ConfigureScheduler();

            bool result = base.OnStart();

            Trace.TraceInformation("MES.Scheduler has been started");

            return result;
        }

        private void ConfigureScheduler()
        {
            try
            {
                var properties = new System.Collections.Specialized.NameValueCollection { { "quartz.threadPool.threadCount", "50" } };
                var schedFact = new StdSchedulerFactory(properties);

                //set scheduler for Nightly jobs
                schedDailyNight = schedFact.GetScheduler();
                schedDailyNight.Start();
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                int hour =00;
                int min = 01;
                var cronScheduleBuilder = CronScheduleBuilder.DailyAtHourAndMinute(hour, min).InTimeZone(timeZoneInfo);
                var jobschedDailyNight = new JobDetailImpl("DailyScheduler", null, typeof(Scheduler.DailyScheduler));

                var triggerschedDailyNight = TriggerBuilder.Create()
                                            .StartNow()
                                            .WithSchedule(cronScheduleBuilder)
                                            .Build();
                //.StartNow()
                //.WithSimpleSchedule(x => x
                //.WithIntervalInMinutes(5)
                //.RepeatForever())
                //.Build();

                schedDailyNight.ScheduleJob(jobschedDailyNight, triggerschedDailyNight);
            }
            catch (Exception ex)
            { }
        }

        public override void OnStop()
        {
            Trace.TraceInformation("MES.Scheduler is stopping");

            schedDailyNight.Shutdown(false);
            schedDailyNight = null;
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("MES.Scheduler has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Replace the following with your own logic.
                while (!cancellationToken.IsCancellationRequested)
                {
                    Trace.TraceInformation("Working");
                    await Task.Delay(30000); //30 sec
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
