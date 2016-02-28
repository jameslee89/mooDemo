using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Controllers;
using Microsoft.Ccr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace LinkenLabs.Market.RestApi.Core
{
    public class NotificationSmsDeliverer
    {
        public static Port<EmptyValue> RunPort = new Port<EmptyValue>();

        public static void Execute(EmptyValue request)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                NotificationSmsDelivery latest = (from n in context.NotificationSmsDeliveries
                                                  orderby n.Created descending
                                                  select n).FirstOrDefault();

                DateTime pointer = DateTime.MinValue;
                if (latest != null)
                {
                    pointer = latest.Created;
                }

                //get all orders create after pointer
                var recentNotifications = (from n in context.Notifications
                                           where n.Created > pointer
                                           orderby n.Created ascending
                                           select n).ToList();

                var webUrl = WebConfigurationManager.AppSettings["WebUrl"];

                var converted = recentNotifications.ConvertAll<NotificationSmsDelivery>(n =>
                {
                    Account account = (from a in context.Accounts
                                       where a.Guid == n.RecipientGuid
                                       select a).First();

                    string lang = "en-US";
                    if (String.IsNullOrEmpty(account.LanguageCode))
                    {
                        lang = account.LanguageCode;
                    }


                    string bodyHtml = NotificationController.GetBodyHtml(n, lang);
                    string bodyPlain = ConvertToSmsText(bodyHtml, webUrl + n.Href, lang);

                    return new NotificationSmsDelivery
                    {
                        Guid = Guid.NewGuid(),
                        RecipientGuid = n.RecipientGuid,
                        Created = n.Created,
                        Sent = DateTime.UtcNow,
                        Body = bodyPlain,
                        Phone = account.Phone,
                        SenderId = "Mooketplace"
                    };
                });

                foreach (var item in converted)
                {
                    if (item.SenderId == "admin")
                    {
                        continue;
                    }

                    PostOffice.PostSmsPort.Post(new SmsMessage
                    {
                        Message = item.Body,
                        PhoneNumber = item.Phone,
                        SenderId = item.SenderId
                    });
                }

                context.NotificationSmsDeliveries.AddRange(converted);
                context.SaveChanges();
            }
        }

        static string ConvertToSmsText(string htmlBody, string href, string lang)
        {
            string pattern = @"<.*?>";

            string plainText = Regex.Replace(htmlBody, pattern, "");

            var googleApiKey = WebConfigurationManager.AppSettings["GoogleApiKey"];

            string shortLink = Google.GetShortUrl(href, googleApiKey);

            string footer = String.Format(Environment.NewLine + "Go to Mooketplace to view {0}", shortLink);

            if (lang == "zh-TW")
            {
                footer = String.Format(Environment.NewLine + "请登錄Mooketplace查看 {0}", shortLink);
            }

            return plainText + footer;
        }

    }
}