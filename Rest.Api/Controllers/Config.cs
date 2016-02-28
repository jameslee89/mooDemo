using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class Config
    {
        public string WebUrl { get; set; }
        public string ApiUrl { get; set; }
        public string FacebookAppId { get; set; }
        public string Version { get; set; }
    }
}