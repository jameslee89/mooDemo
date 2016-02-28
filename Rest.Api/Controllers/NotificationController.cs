using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Core;
using LinkenLabs.Market.RestApi.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class NotificationController : ApiController
    {
        [BasicAuthentication]
        public IEnumerable<NotificationView> Get(string lang = "en-US")
        {
            Account account = AccountController.GetAccountByUsername(User.Identity.Name);

            //get all notifications for this user

            using (DatabaseContext context = new DatabaseContext())
            {
                var notifications = (from n in context.Notifications
                                     where n.RecipientGuid == account.Guid
                                     orderby n.Created descending
                                     select n).ToList();

                var webUrl = WebConfigurationManager.AppSettings["WebUrl"];


                var converted = notifications.ConvertAll<NotificationView>((n) =>
                {
                    return new NotificationView
                    {
                        Created = DateTime.SpecifyKind(n.Created, DateTimeKind.Utc),
                        Guid = n.Guid,
                        RecipientGuid = n.RecipientGuid,
                        SenderGuid = n.SenderGuid,
                        SenderPictureUrl = GetSenderPictureUrl(n),
                        Type = n.Type,
                        Url = webUrl + n.Href,
                        IsRead = n.IsRead,
                        IsHidden = n.IsHidden,
                        BodyHtml = GetBodyHtml(n, lang)
                    };
                });

                return converted;
            }
        }

        public static string GetSenderPictureUrl(Notification notification)
        {
            var apiUrl = WebConfigurationManager.AppSettings["BaseUrl"];

            switch (notification.Type)
            {
                case TradeOfferCreatedNotifier.TradeOfferSent:
                    return String.Format("{0}/v1/account/{1}/picture", apiUrl, Guid.Empty);
                default:
                    return String.Format("{0}/v1/account/{1}/picture", apiUrl, notification.SenderGuid);
            }
        }

        public static string GetBodyHtml(Notification notification, string lang)
        {
            switch (notification.Type)
            {
                case OrderCommentNotifier.CommentReceived:
                    return GetOrderCommentReceivedBodyHtml(notification, lang);
                case OrderCommentNotifier.CommentReplied:
                    return GetOrderCommentRepliedBodyHtml(notification, lang);
                case TradeOfferCreatedNotifier.TradeOfferSent:
                    return GetTradeOfferSentBodyHtml(notification, lang);
                case TradeOfferCreatedNotifier.TradeOfferReceived:
                    return GetTradeOfferReceivedBodyHtml(notification, lang);
                case TradeOfferActionNotifier.TradeOfferAccepted:
                    return GetTradeOfferAcceptedBodyHtml(notification, lang);
                case TradeOfferActionNotifier.TradeOfferCancelled:
                    return GetTradeOfferCancelledBodyHtml(notification, lang);
                case TradeOfferExpiryNotifier.OfferExpiredOrdererNotification:
                    return GetTradeOfferExpiredOrdererBodyHtml(notification, lang);
                case TradeOfferExpiryNotifier.OfferExpiredPosterNotification:
                    return GetTradeOfferExpiredPosterBodyHtml(notification, lang);
                default:
                    throw new Exception();
            }
        }

        static string GetTradeOfferExpiredPosterBodyHtml(Notification notification, string lang)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                //get tradeOffer Guid
                string[] array = notification.Href.Split('/');
                Guid tradeOfferGuid = Guid.Parse(array[array.Length - 1]);

                TradeOffer offer = (from o in context.TradeOffers
                                    where o.Guid == tradeOfferGuid
                                    select o).First();

                string bodyHtml = String.Format("Order Cancellation {0}. This order was not confirmed and is now cancelled.", offer.TradeCode);
                if (lang == "zh-TW")
                {
                    bodyHtml = String.Format("訂單取消{0}。這個訂單沒有得到確認，現在已取消了", offer.TradeCode);
                }
                return bodyHtml;
            }
        }

        static string GetTradeOfferExpiredOrdererBodyHtml(Notification notification, string lang)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                //get tradeOffer Guid
                string[] array = notification.Href.Split('/');
                Guid tradeOfferGuid = Guid.Parse(array[array.Length - 1]);

                TradeOffer offer = (from o in context.TradeOffers
                                    where o.Guid == tradeOfferGuid
                                    select o).First();

                Account poster = (from a in context.Accounts
                                  where a.Guid == offer.ToAccountGuid
                                  select a).First();

                string bodyHtml = String.Format("Order Cancellation {0}. Your order was not confirmed by <b>{1}</b> and is now cancelled.", offer.TradeCode, poster.Username);
                if (lang == "zh-TW")
                {
                    bodyHtml = String.Format("訂單取消{0}。您的訂單沒有被<b>{1}</b>確認，現在取消了", offer.TradeCode, poster.Username);
                }
                return bodyHtml;
            }
        }

        static string GetTradeOfferCancelledBodyHtml(Notification notification, string lang)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var sender = (from a in context.Accounts
                              where a.Guid == notification.SenderGuid
                              select a).FirstOrDefault();

                //get tradeOffer Guid
                string[] array = notification.Href.Split('/');
                Guid tradeOfferGuid = Guid.Parse(array[array.Length - 1]);

                TradeOffer offer = (from o in context.TradeOffers
                                    where o.Guid == tradeOfferGuid
                                    select o).First();

                string bodyHtml = String.Format("Order Cancellation {0}. Unfortunately {1} has cancelled this order.", offer.TradeCode, sender.Username);
                if (lang == "zh-TW")
                {
                    bodyHtml = String.Format("訂單取消{0}。{1}取消了此訂單", offer.TradeCode, sender.Username);
                }
                return bodyHtml;
            }
        }

        static string GetTradeOfferAcceptedBodyHtml(Notification notification, string lang)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                //get tradeOffer Guid
                string[] array = notification.Href.Split('/');
                Guid tradeGuid = Guid.Parse(array[array.Length - 1]);

                Trade trade = (from t in context.Trades
                               where t.Guid == tradeGuid
                               select t).First();

                string bodyHtml = String.Format("Order Confirmation {0}. Your order is confirmed", trade.TradeCode);
                if (lang == "zh-TW")
                {
                    bodyHtml = String.Format("訂單確認{0}.您的訂單被確認", trade.TradeCode);
                }
                return bodyHtml;
            }
        }

        static string GetTradeOfferReceivedBodyHtml(Notification notification, string lang)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var sender = (from a in context.Accounts
                              where a.Guid == notification.SenderGuid
                              select a).FirstOrDefault();

                //get tradeOffer Guid
                string[] array = notification.Href.Split('/');
                Guid tradeOfferGuid = Guid.Parse(array[array.Length - 1]);

                TradeOffer offer = (from o in context.TradeOffers
                                    where o.Guid == tradeOfferGuid
                                    select o).First();

                string bodyHtml = String.Format("Order Received {1}. Nice! <b>{0}</b> just ordered your product. Please confirm this order within 24 hours", sender.Username, offer.TradeCode);
                if (lang == "zh-TW")
                {
                    bodyHtml = String.Format("收到訂單{1}. {0}剛剛訂購你的產品。請確認24小時內這個訂單", sender.Username, offer.TradeCode);
                }
                return bodyHtml;
            }
        }

        static string BuildBodyHtml(Func<Account, string> bodyBuilder, Notification notification)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var sender = (from a in context.Accounts
                              where a.Guid == notification.SenderGuid
                              select a).First();

                return bodyBuilder(sender);
            }
        }

        static string GetTradeOfferSentBodyHtml(Notification notification, string lang)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                //get tradeOffer Guid
                string[] array = notification.Href.Split('/');
                Guid tradeOfferGuid = Guid.Parse(array[array.Length - 1]);

                TradeOffer offer = (from o in context.TradeOffers
                                    where o.Guid == tradeOfferGuid
                                    select o).First();

                string bodyHtml = String.Format("Order Received {0}. Thanks for your order! A confirmation will be sent within 24 hours", offer.TradeCode);

                if (lang == "zh-TW")
                {
                    bodyHtml = String.Format("訂單收到{0}. 感謝您的訂單！確認將在24小時內發出", offer.TradeCode);
                }

                return bodyHtml;
            }
        }

        static string GetOrderCommentRepliedBodyHtml(Notification notification, string lang)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var sender = (from a in context.Accounts
                              where a.Guid == notification.SenderGuid
                              select a).First();

                string bodyHtml = String.Format("<b>{0}</b> replied to a post you commented on", sender.Username);

                if (lang == "zh-TW")
                {
                    bodyHtml = String.Format("<b>{0}</b>回复了您留言過的產品", sender.Username);
                }

                return bodyHtml;
            }
        }

        static string GetOrderCommentReceivedBodyHtml(Notification notification, string lang)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var sender = (from a in context.Accounts
                              where a.Guid == notification.SenderGuid
                              select a).First();

                string bodyHtml = String.Format("<b>{0}</b> commented on your post", sender.Username);

                if (lang == "zh-TW")
                {
                    bodyHtml = String.Format("<b>{0}</b>留言在您的產品", sender.Username);
                }

                return bodyHtml;
            }


        }



    }
}