using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.schedules
{
    public class Scheduler
    {
        private static Scheduler _instance = new Scheduler();
        public static Scheduler Instance
        {
            get
            {
                return _instance;
            }
        }

        private IScheduler scheduler = new StdSchedulerFactory().GetScheduler();
        TimeZoneInfo chinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

        public void Init()
        {
            Schedule<CreateTradeReviewJob>("0 0/1 * 1/1 * ? *");
            Schedule<AccountRatingJob>("0 0/1 * 1/1 * ? *");
        }

        public void Schedule<T>(string cronSchedule) where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(typeof(T).Name, typeof(T).Name + "Group")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(typeof(T).Name + "Trigger", typeof(T).Name + "Group")
                .WithCronSchedule(cronSchedule, x => x
                    .InTimeZone(chinaTimeZone))
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        private Scheduler()
        {
            scheduler.Start();
        }
    }
}