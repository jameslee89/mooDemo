using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ConversationInfo
    {
        public Guid With { get; set; }
        public string WithName { get; set; }
        public bool IsReceived { get; set; } //whether this message was from other party or not
        public int UnreadCount { get; set; }
        public DateTime Created { get; set; }
        public string LastMessage { get; set; }
    }
}