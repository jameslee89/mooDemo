using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class VerifyMobileRequest
    {
        public string MobileNumber { get; set; }
        public string Code { get; set; }
    }
}