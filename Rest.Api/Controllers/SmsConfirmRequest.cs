using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class SmsConfirmRequest
    {
        public string LanguageCode { get; set; }
        public string MobileNumber { get; set; }
    }
}