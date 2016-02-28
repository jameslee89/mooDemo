using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Mail;
using LinkenLabs.Market.RestApi.Filters;
using LinkenLabs.Market.RestApi.Core;
using LinkenLabs.Market.Core;

namespace LinkenLabs.Market.RestApi.Controllers
{
    [BasicAuthentication]
    public class NewProductController : ApiController
    {
        public void Post(NewProductRequest[] request)
        {
            string from = WebApplication.Instance.SmtpSettings.Username;
            string to = WebApplication.Instance.SupportEmail;
            string body = JsonConvert.SerializeObject(request);

            MailMessage message = new MailMessage(from, to);
            message.Subject = String.Format("New Product Request from {0}", User.Identity.Name);
            message.Body = body;
            message.IsBodyHtml = false;

            PostOffice.PostMailPort.Post(message);
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/newproduct/productorder")]
        public void Post(NewProductOrderRequest request)
        {
            string from = WebApplication.Instance.SmtpSettings.Username;
            string to = WebApplication.Instance.SupportEmail;
            string body = JsonConvert.SerializeObject(request);

#if DEBUG
            System.Diagnostics.Debug.WriteLine(body);

#else
            MailMessage message = new MailMessage(from, to);
            message.Subject = String.Format("New Product-Order Request from {0}", request.UserName);
            message.Body = body;
            message.IsBodyHtml = false;
            PostOffice.PostMailPort.Post(message);
#endif


        }

    }


}