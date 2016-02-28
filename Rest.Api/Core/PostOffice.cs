using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Controllers;
using Microsoft.Ccr.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;

namespace LinkenLabs.Market.RestApi.Core
{
    public class PostOffice
    {
        public static Port<NewTradeMessage> NewTradePort = new Port<NewTradeMessage>();
        public static Port<MailMessage> PostMailPort = new Port<MailMessage>();
        public static Port<SmsMessage> PostSmsPort = new Port<SmsMessage>();

        public static void HandleNewError(NewErrorMessage msg)
        {
            Exception exception = msg.Exception;

            string from = WebApplication.Instance.SmtpSettings.Username;
            string to = WebApplication.Instance.SupportEmail;
            string body = "<b>Message</b>: " + exception.Message +
                          "</br><b>InnerException</b>: " + exception.InnerException +
                          "</br><b>Source</b>: " + exception.Source +
                          "</br><b>StackTrace</b>: " + exception.StackTrace;

            MailMessage message = new MailMessage(from, to);
            message.Subject = String.Format("ERROR has occurred in Mooketplace");
            message.Body = body;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;

            PostMailPort.Post(message);
        }

        public static void HandleSmsMessage(SmsMessage message)
        {
            SmsUtil.SendMessage(message.PhoneNumber, message.Message, message.SenderId);
        }


        public static void HandlePostMessage(MailMessage message)
        {
            using (SmtpClient smtpClient = SmtpUtil.CreateSmtpClient())
            {
                smtpClient.Send(message);
            }
        }

        static void SendEmail(MailMessage msg)
        {
            SmtpClient smtpClient = SmtpUtil.CreateSmtpClient();
            smtpClient.SendAsync(msg, null);
        }

        public static void HandleNewTrade(NewTradeMessage msg)
        {
            //send email
            using (DatabaseContext context = Util.CreateContext())
            {
                Trade trade = (from t in context.Trades
                               where t.Guid == msg.TradeGuid
                               select t).First();

                Order order = (from o in context.Orders
                               where o.Guid == trade.OrderGuid
                               select o).First();

                //seller SMS
                Account seller = (from a in context.Accounts
                                  where a.Guid == order.CreatedByAccountGuid
                                  select a).First();

                string buyerPhone = trade.BuyerPhoneNumber;
                string sellerName = seller.Username;
                string sellerPhone = seller.Phone;

                //inactive true since this item may be just sold out, making it inactive
                SkuView skuView = new SkuViewController().GetList(skus: trade.SkuGuid.ToString(), showInactive: true).First();

                string sellerNotice = GetSellerTradeNotice(trade, skuView, "en-US");
                PostSmsPort.Post(new SmsMessage
                {
                    Message = sellerNotice,
                    PhoneNumber = sellerPhone,
                    SenderId = "mooketplace"
                });

                string buyerNotice = GetBuyerTradeNotice(trade, skuView, seller.Username, seller.Phone);
                PostSmsPort.Post(new SmsMessage
                {
                    Message = buyerNotice,
                    PhoneNumber = trade.BuyerPhoneNumber,
                    SenderId = "mooketplace"
                });

                SendSupportEmail(trade, skuView, seller.Username, seller.Phone);
            }
        }

        static void SendSupportEmail(Trade trade, SkuView sku, string sellerName, string sellerPhone)
        {
            string templateBody = GetTemplateBody("~/templates/tradeNotice.txt", "en-US");
            templateBody = templateBody.Replace("{TradeCode}", trade.TradeCode);
            templateBody = templateBody.Replace("{Quantity}", trade.Quantity.ToString());
            templateBody = templateBody.Replace("{SkuName}", sku.FullName);
            templateBody = templateBody.Replace("{Total}", trade.Total.ToString());
            templateBody = templateBody.Replace("{Currency}", trade.Currency);
            templateBody = templateBody.Replace("{BuyerName}", trade.BuyerUserName);
            templateBody = templateBody.Replace("{BuyerPhone}", trade.BuyerPhoneNumber);
            templateBody = templateBody.Replace("{SellerName}", sellerName);
            templateBody = templateBody.Replace("{SellerPhone}", sellerPhone);

            MailMessage message = new MailMessage(WebApplication.Instance.SmtpSettings.Username, WebApplication.Instance.SupportEmail);
            message.Subject = String.Format("Trade Notice {0}", trade.TradeCode);
            message.Body = templateBody;
            message.IsBodyHtml = false;
            SmtpClient smtpClient = SmtpUtil.CreateSmtpClient();
            smtpClient.SendAsync(message, null);
        }

        public static void SendBuyerFeedbackSms(Trade trade, SkuView sku)
        {
            string feedbackUrl = String.Format(@"{0}/zh-TW/buyerfeedback/{1}", WebApplication.Instance.WebUrl, trade.Guid);
            string shortUrl = Google.GetShortUrl(feedbackUrl, WebApplication.Instance.GoogleApiKey);

            string smsTemplate = GetBuyerFeedbackSmsTemplate(trade.BuyerLanguageCode);

            smsTemplate = smsTemplate.Replace("{CustomerName}", trade.BuyerUserName);
            smsTemplate = smsTemplate.Replace("{SkuName}", sku.FullName);
            smsTemplate = smsTemplate.Replace("{Link}", shortUrl);

            SmsUtil.SendMessage(trade.BuyerPhoneNumber, smsTemplate, "mooketplace");
        }

        static string GetBuyerTradeNotice(Trade trade, SkuView sku, string sellerName, string sellerPhone)
        {
            string smsTemplate = GetBuyerSmsNoticeTemplate(trade.BuyerLanguageCode);
            smsTemplate = smsTemplate.Replace("{TradeCode}", trade.TradeCode);
            smsTemplate = smsTemplate.Replace("{Quantity}", trade.Quantity.ToString());
            smsTemplate = smsTemplate.Replace("{SkuName}", sku.FullName);
            smsTemplate = smsTemplate.Replace("{Total}", trade.Total.ToString());
            smsTemplate = smsTemplate.Replace("{Currency}", trade.Currency);
            smsTemplate = smsTemplate.Replace("{SellerName}", sellerName);
            smsTemplate = smsTemplate.Replace("{SellerNumber}", sellerPhone);
            return smsTemplate;
        }

        static string GetSellerTradeNotice(Trade trade, SkuView sku, string lang)
        {
            string smsTemplate = GetStoreBuyNoticeTemplate(lang);
            smsTemplate = smsTemplate.Replace("{TradeCode}", trade.TradeCode);
            smsTemplate = smsTemplate.Replace("{Name}", trade.BuyerUserName);
            smsTemplate = smsTemplate.Replace("{Number}", trade.BuyerPhoneNumber);
            smsTemplate = smsTemplate.Replace("{Quantity}", trade.Quantity.ToString());
            smsTemplate = smsTemplate.Replace("{SkuName}", sku.FullName);
            smsTemplate = smsTemplate.Replace("{Total}", trade.Total.ToString());
            smsTemplate = smsTemplate.Replace("{Currency}", trade.Currency);
            return smsTemplate;
        }

        static string GetBuyerFeedbackSmsTemplate(string lang)
        {
            return GetTemplateBody(@"~/templates/BuyerFeedbackSms.txt", lang);
        }

        static string GetBuyerSmsNoticeTemplate(string lang)
        {
            return GetTemplateBody(@"~/templates/BuyerSmsNotice.txt", lang);
        }

        static string GetStoreBuyNoticeTemplate(string lang)
        {
            return GetTemplateBody(@"~/templates/storeBuyNotice.txt", lang);
        }

        static string GetBuyConfirmationTemplate(string lang)
        {
            return GetTemplateBody(@"~/templates/buyConfirm.html", lang);
        }

        static string GetTemplateBody(string virtualPath, string lang)
        {
            IDictionary<string, object> dictionary = TranslationsUtil.GetTranslationDictionary(lang);

            string templateBody = File.ReadAllText(HostingEnvironment.MapPath(virtualPath));

            foreach (var key in dictionary.Keys)
            {

                if (dictionary[key] is string)
                {
                    templateBody = templateBody.Replace("{" + key + "}", (string)dictionary[key]);
                }
                else
                {
                    IDictionary<string, object> subDictionary = (IDictionary<string, object>)dictionary[key];
                    foreach (var subKey in subDictionary.Keys)
                    {
                        templateBody = templateBody.Replace("{" + key + "." + subKey + "}", (string)subDictionary[subKey]);
                    }
                }

            }

            return templateBody;
        }
    }
}