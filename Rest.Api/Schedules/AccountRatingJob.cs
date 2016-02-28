using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Core;
using Microsoft.Ccr.Core;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.schedules
{
    [DisallowConcurrentExecution]
    public class AccountRatingJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            AccountRatingGenerator.RunPort.Post(EmptyValue.SharedInstance);
        }

    }
}