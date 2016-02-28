using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkenLabs.Market.RestApi.Core
{
    public class SmsMessage
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string SenderId { get; set; }
    }
}
