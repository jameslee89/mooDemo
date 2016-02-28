using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class AccountInfo
    {
        public Guid Guid { get; set; }
        public string Username { get; set; }
        public string FacebookUserId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }
}