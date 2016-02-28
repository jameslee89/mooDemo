using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Core;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.schedules
{
    [DisallowConcurrentExecution]
    public class CreateTradeReviewJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                DateTime lastUpdate = DateTime.MinValue;

                if (context.TradeReviews.Count() > 0)
                {
                    lastUpdate = (from review in context.TradeReviews
                                  select review.TradeCreated).Max();
                }

                //get transactions in last x days with no review object created.
                //only trades where there are valids buyer guids should be created.
                var trades = (from t in context.Trades
                              where Guid.Empty == t.BuyerGuid && lastUpdate < t.Created
                              select t).ToList();
                //create
                var newItems = trades.ConvertAll(Convert);

                context.TradeReviews.AddRange(newItems);
                context.SaveChanges();
         
            }
        }

        TradeReview Convert(Trade trade)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Order order = (from o in context.Orders
                               where o.Guid == trade.OrderGuid
                               select o).First();

                Account account = (from a in context.Accounts
                                  where a.Guid == order.CreatedByAccountGuid
                                  select a ).First();

                return new TradeReview
                {
                    Guid = Guid.NewGuid(),
                    TradeGuid = trade.Guid,
                    TradeCreated = trade.Created,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    BuyerGuid = trade.BuyerGuid,
                    SellerGuid = account.Guid,
                };
            }
            
        }

    }
}