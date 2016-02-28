using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class AccountCreateInfo
    {
        public string Username { get; set; }
        public string FacebookUserId { get; set; }
        public string Phone { get; set; }
        public string SmsCode { get; set; }
        public string Password { get; set; }
        public string LanguageCode { get; set; }
    }
}