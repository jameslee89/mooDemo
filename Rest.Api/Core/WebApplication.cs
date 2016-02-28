using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Core
{
    public class WebApplication
    {
        private static WebApplication _instance = new WebApplication();

        public static WebApplication Instance
        {
            get
            {
                return _instance;
            }
        }


        public SmtpSettings SmtpSettings { get; private set; }
        public string SupportEmail { get; set; }
        public string SalesEmail { get; set; }
        public string GoogleApiKey { get; set; }
        public string BaseUrl { get; set; }
        public string WebUrl { get; set; }
        public string FacebookAppId { get; set; }
        public string FacebookSecret { get; set; }

        private WebApplication()
        {
            SmtpSettings = new SmtpSettings();
        }
        
    }
}