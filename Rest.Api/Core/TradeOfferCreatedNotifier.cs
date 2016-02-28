using LinkenLabs.Market.Core;
using Microsoft.Ccr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Core
{
    public class TradeOfferCreatedNotifier
    {
        public const string TradeOfferSent = "TradeOffer:Sent"; //for sender
        public const string TradeOfferReceived = "TradeOffer:Received"; //for receiver


        public static Port<EmptyValue> RunPort = new Port<EmptyValue>();

        public static void Execute(EmptyValue request)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                string[] types = new string[] { TradeOfferSent, TradeOfferReceived };
                var lastNotification = (from n in context.Notifications
                                        where types.Contains(n.Type)
                                        orderby n.Created descending
                                        select n).FirstOrDefault();

                DateTime pointer = DateTime.MinValue;
                if (lastNotification != null)
                {
                    pointer = lastNotification.Created;
                }


                //select all trades since the last notification
                var newOffers = (from o in context.TradeOffers
                                 where o.Created > pointer
                                 orderby o.Created ascending
                                 select o).ToList();

                List<Notification> notifications = new List<Notification>();

                foreach (var offer in newOffers)
                {
                    //create 
                    Account from = (from a in context.Accounts
                                    where a.Guid == offer.FromAccountGuid
                                    select a).First();

                    Account to = (from a in context.Accounts
                                  where a.Guid == offer.ToAccountGuid
                                  select a).First();


                    Notification receivedNotification = new Notification
                    {
                        Guid = Guid.NewGuid(),
                        Created = offer.Created,
                        RecipientGuid = offer.ToAccountGuid,
                        SenderGuid = offer.FromAccountGuid,
                        Type = TradeOfferReceived,
                        Href = String.Format("/{0}/tradeOffer/{1}", to.LanguageCode, offer.Guid)
                    };

                    notifications.Add(receivedNotification);

                    //create 
                    Notification sentNotification = new Notification
                    {
                        Guid = Guid.NewGuid(),
                        Created = offer.Created,
                        RecipientGuid = offer.FromAccountGuid,
                        SenderGuid = Guid.Empty,
                        Type = TradeOfferSent,
                        Href = String.Format("/{0}/tradeOffer/{1}", from.LanguageCode, offer.Guid)
                    };

                    notifications.Add(sentNotification);
                }

                context.Notifications.AddRange(notifications);
                context.SaveChanges();

                NotificationSmsDeliverer.RunPort.Post(new EmptyValue());
            }
        }

        static string GetHref(Guid recipientGuid, Guid tradeGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Account recipient = (from a in context.Accounts
                                     where a.Guid == recipientGuid
                                     select a).First();

                var recipientLang = !String.IsNullOrEmpty(recipient.LanguageCode) ? recipient.LanguageCode : "en-US";

                return String.Format("/{0}/trade/{1}", recipientLang, tradeGuid);
            }

        }
    }
}