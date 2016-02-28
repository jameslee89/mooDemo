using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class RegistrationRequest
    {
        public string Name { get; set; }
        public string PhoneOrEmail { get; set; }
    }
}