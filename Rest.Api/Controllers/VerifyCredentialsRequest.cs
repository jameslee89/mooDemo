using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class VerifyCredentialsRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}