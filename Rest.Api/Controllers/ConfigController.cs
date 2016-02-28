using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ConfigController : ApiController
    {
        // GET api/<controller>
        public Config Get()
        {
            var baseUrl = WebConfigurationManager.AppSettings["BaseUrl"];
            var webUrl = WebConfigurationManager.AppSettings["WebUrl"];
            var facebookApiId = WebConfigurationManager.AppSettings["FacebookAppId"];
            var version = WebConfigurationManager.AppSettings["Version"];

            return new Config
            {
                ApiUrl = baseUrl,
                WebUrl = webUrl,
                FacebookAppId = facebookApiId,
                Version = version
            };
        }
    }
}