using LinkenLabs.Market.Core;
using Microsoft.Ccr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Core
{
    public class TradeOfferActionNotifier
    {
        public const string TradeOfferAccepted = "TradeOffer:Accepted"; //for sender
        public const string TradeOfferCancelled = "TradeOffer:Cancelled"; //for sender

        public static Port<EmptyValue> RunPort = new Port<EmptyValue>();

        public static void Execute(EmptyValue request)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                string[] types = new string[] { TradeOfferAccepted, TradeOfferCancelled };
                var lastNotification = (from n in context.Notifications
                                        where types.Contains(n.Type)
                                        orderby n.Created descending
                                        select n).FirstOrDefault();

                DateTime pointer = DateTime.MinValue;
                if (lastNotification != null)
                {
                    pointer = lastNotification.Created;
                }


                //select all accepted and cancelled offers since last
                var newOffers = (from o in context.TradeOffers
                                 where o.LastModified > pointer 
                                 && (o.IsAccepted || o.IsCancelled)
                                 orderby o.LastModified ascending
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
                    if (offer.IsAccepted)
                    {
                        Trade trade = (from t in context.Trades
                                       where t.TradeCode == offer.TradeCode
                                       select t).First(); 

                        Notification acceptNotification = new Notification
                        {
                            Guid = Guid.NewGuid(),
                            Created = offer.LastModified,
                            RecipientGuid = offer.FromAccountGuid,
                            SenderGuid = Guid.Empty,
                            Type = TradeOfferAccepted,
                            Href = String.Format("/{0}/trade/{1}", from.LanguageCode, trade.Guid)
                        };

                        notifications.Add(acceptNotification);
                    }

                    if (offer.IsCancelled)
                    {
                        Notification cancelNotification = new Notification
                        {
                            Guid = Guid.NewGuid(),
                            Created = offer.LastModified,
                            RecipientGuid = offer.FromAccountGuid,
                            SenderGuid = Guid.Empty,
                            Type = TradeOfferCancelled,
                            Href = String.Empty
                        };

                        notifications.Add(cancelNotification);
                    }
                }

                context.Notifications.AddRange(notifications);
                context.SaveChanges();

                NotificationSmsDeliverer.RunPort.Post(new EmptyValue());
            }
        }
    }
}