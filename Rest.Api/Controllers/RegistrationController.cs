using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class RegistrationController : ApiController
    {
        /// <summary>
        /// Logs a registration request
        /// </summary>
        /// <param name="request"></param>
        public void Post(RegistrationRequest request)
        {
            string body = JsonConvert.SerializeObject(request);
            string from = WebApplication.Instance.SmtpSettings.Username;
            string to = WebApplication.Instance.SalesEmail;
            MailMessage mailMessage = new MailMessage(from, to, "New Prospect", body);
            PostOffice.PostMailPort.Post(mailMessage);
        }

    }
}