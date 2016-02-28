using LinkenLabs.Market.Core;
using Microsoft.Ccr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Core
{
    //creates notifications from ordercomments
    public class OrderCommentNotifier
    {
        public const string CommentReceived = "OrderComment:Received";
        public const string CommentReplied = "OrderComment:Replied";

        public static Port<EmptyValue> RunPort = new Port<EmptyValue>();

        public static void Execute(EmptyValue request)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var lastNotification = (from n in context.Notifications
                                        where n.Type == CommentReceived || n.Type == CommentReplied
                                        orderby n.Created descending
                                        select n).FirstOrDefault();

                DateTime pointer = DateTime.MinValue;
                if (lastNotification != null)
                {
                    pointer = lastNotification.Created;
                }

                //get all orders create after pointer
                var recentComments = (from c in context.Comments
                                      where c.Created > pointer
                                      orderby c.Created ascending
                                      select c).ToList();


                List<Notification> newNotifications = new List<Notification>();

                foreach (var comment in recentComments)
                {
                    Order order = (from o in context.Orders
                                   where o.Guid == comment.OrderGuid
                                   select o).First();

                    Account orderCreator = (from a in context.Accounts
                                            where a.Guid == order.CreatedByAccountGuid
                                            select a).First();

                    if (comment.CreatedBy == order.CreatedByAccountGuid) //user commented on his own order
                    {
                        //notify all previous commenters
                        //get all commenters on the order other than the user
                        var commenterGuids = (from c in context.Comments
                                              where c.OrderGuid == order.Guid && c.CreatedBy != orderCreator.Guid
                                              select c.CreatedBy).Distinct().ToList();


                        var notifications = commenterGuids.ConvertAll<Notification>(guid =>
                        {
                            //get user
                            Account recipient = (from a in context.Accounts
                                                 where a.Guid == guid
                                                 select a).First();

                            string lang = !String.IsNullOrEmpty(recipient.LanguageCode) ? recipient.LanguageCode : "en-US";

                            Notification newNotification = new Notification
                            {
                                Guid = Guid.NewGuid(),
                                Created = comment.Created,
                                SenderGuid = comment.CreatedBy,
                                RecipientGuid = recipient.Guid,
                                Type = CommentReplied,
                                Href = String.Format("/{0}/order/{1}", lang, order.Guid) //use relative since webserver might move
                            };
                            return newNotification;
                        });

                        newNotifications.AddRange(notifications);
                    }
                    else
                    {
                        string lang = !String.IsNullOrEmpty(orderCreator.LanguageCode) ? orderCreator.LanguageCode : "en-US";  //some accounts may not have prefered language, i.e. admin

                        Notification newNotification = new Notification
                        {
                            Guid = Guid.NewGuid(),
                            Created = comment.Created,
                            SenderGuid = comment.CreatedBy,
                            RecipientGuid = order.CreatedByAccountGuid,
                            Type = CommentReceived,
                            Href = String.Format("/{0}/order/{1}", lang, order.Guid) //use relative since webserver might move
                        };
                        newNotifications.Add(newNotification);
                    }

                }

                context.Notifications.AddRange(newNotifications);
                context.SaveChanges();

                NotificationSmsDeliverer.RunPort.Post(new EmptyValue());
            }
        }
    }
}