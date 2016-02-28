using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ConversationMessage
    {
        public DateTime Created { get; set; }
        public string From { get; set; }
        public Guid OrderGuid { get; set; }
        public string Body { get; set; }
    }
}