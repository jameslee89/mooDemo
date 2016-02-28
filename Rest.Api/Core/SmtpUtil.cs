using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace LinkenLabs.Market.RestApi.Core
{
    public class SmtpUtil
    {
        public static SmtpClient CreateSmtpClient()
        {
            SmtpSettings settings = WebApplication.Instance.SmtpSettings;

            SmtpClient smtpClient = new SmtpClient(settings.Host, settings.Port);
            smtpClient.EnableSsl = settings.EnableSsl;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(settings.Username, settings.Password);
            return smtpClient;
        }

        public static void SendSupportEmail(Exception ex)
        {
            SmtpClient smtpClient = CreateSmtpClient();
            smtpClient.Send(new MailMessage(WebApplication.Instance.SmtpSettings.Username, WebApplication.Instance.SupportEmail, "Mooketplace Failure", ex.ToString()));
        }
    }
}