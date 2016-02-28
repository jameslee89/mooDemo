using LinkenLabs.Market.Core;
using Microsoft.Ccr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Core
{
    public class TradeOfferExpiryNotifier
    {
        public const string OfferExpiredPosterNotification = "TradeOffer:Expired:Poster"; //for poster
        public const string OfferExpiredOrdererNotification = "TradeOffer:Expired:Orderer"; //for orderer

        public static Port<EmptyValue> RunPort = new Port<EmptyValue>();

        public static void Execute(EmptyValue request)
        {
            DateTime startTime = DateTime.UtcNow;
            DateTime pointer = GetPointer();

            using (DatabaseContext context = new DatabaseContext())
            {
                //get expired offers
                var tradeOffers = (from o in context.TradeOffers
                                   where !o.IsCancelled && !o.IsAccepted && DateTime.UtcNow > o.Deadline
                                   && o.Deadline > pointer
                                   select o).ToList();

                List<Notification> newNotifications = new List<Notification>();
                //convert to notifications
                foreach (var item in tradeOffers)
                {
                    Account from = (from a in context.Accounts
                                    where a.Guid == item.FromAccountGuid
                                    select a).First();

                    Account to = (from a in context.Accounts
                                  where a.Guid == item.ToAccountGuid
                                  select a).First();

                    //send to orderer
                    Notification ordererNotification = new Notification
                    {
                        Guid = Guid.NewGuid(),
                        Created = startTime,
                        Href = String.Format(@"/{0}/tradeOffer/{1}", from.LanguageCode, item.Guid),
                        RecipientGuid = from.Guid,
                        SenderGuid = Guid.Empty,
                        Type = OfferExpiredOrdererNotification
                    };

                    newNotifications.Add(ordererNotification);

                    Notification posterNotification = new Notification
                    {
                        Guid = Guid.NewGuid(),
                        Created = startTime,
                        Href = String.Format(@"/{0}/tradeOffer/{1}", from.LanguageCode, item.Guid),
                        RecipientGuid = to.Guid,
                        SenderGuid = Guid.Empty,
                        Type = OfferExpiredPosterNotification
                    };

                    newNotifications.Add(posterNotification);
                }

                context.Notifications.AddRange(newNotifications);
                context.SaveChanges();

                NotificationSmsDeliverer.RunPort.Post(new EmptyValue());
            }
        }

        static DateTime GetPointer()
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var lastNotification = (from n in context.Notifications
                                        where n.Type == OfferExpiredPosterNotification
                                        || n.Type == OfferExpiredOrdererNotification
                                        orderby n.Created descending
                                        select n).FirstOrDefault();

                DateTime pointer = DateTime.MinValue;
                if (lastNotification != null)
                {
                    pointer = lastNotification.Created;
                }

                return pointer;
            }
        }
    }
}