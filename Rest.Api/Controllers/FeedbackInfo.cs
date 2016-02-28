using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class FeedbackInfo
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public Guid ReviewerGuid { get; set; }
        public string ReviewerName { get; set; }
        public Guid RevieweeGuid { get; set; }
        public string RevieweeName { get; set; }
        public DateTime Created { get; set; }
        

    }
}