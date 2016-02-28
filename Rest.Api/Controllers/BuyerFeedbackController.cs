//using LinkenLabs.Market.Core;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace LinkenLabs.Market.RestApi.Controllers
//{
//    public class BuyerFeedbackController : ApiController
//    {

//        [AcceptVerbs("GET")]
//        public BuyerFeedbackStatus Get(Guid id)
//        {
//            using (DatabaseContext context = Util.CreateContext())
//            {
//                var feedback = (from f in context.BuyerFeedback.AsNoTracking()
//                                where f.TradeGuid == id
//                                select f).FirstOrDefault();

//                if (feedback == null)
//                {
//                    var trade = (from t in context.Trades.AsNoTracking()
//                                 where t.Guid == id
//                                 select t).FirstOrDefault();

//                    if (trade == null)
//                    {
//                        return new BuyerFeedbackStatus
//                        {
//                            Status = "Does not exist"
//                        };
//                    }

//                    return new BuyerFeedbackStatus
//                    {
//                        BuyerName = trade.BuyerFullName,
//                        Status = "Request not sent"
//                    };
//                }

//                return new BuyerFeedbackStatus
//                {
//                    BuyerName = feedback.BuyerName,
//                    Status = feedback.Status
//                };
//            }
//        }



//        [AcceptVerbs("POST")]
//        public void Post(Guid id, BuyerFeedbackInfo model)
//        {
//            using (DatabaseContext context = Util.CreateContext())
//            {

//                //get the trade
//                var trade = (from t in context.Trades.AsNoTracking()
//                             where t.Guid == id
//                             select t).First();

//                TradeReview newFeedback = new TradeReview
//                {
//                    BuyerName = trade.BuyerFullName,
//                    Comment = model.Comment,
//                    Rating = model.Rating,
//                    TradeGuid = id,
//                    Guid = Guid.NewGuid(),
//                    Created = DateTime.UtcNow,
//                    Status = model.Status
//                };

//                context.BuyerFeedback.Add(newFeedback);
//                context.SaveChanges();
//            }
//        }

//        [AcceptVerbs("PUT")]
//        public void Put(Guid id, BuyerFeedbackInfo model)
//        {
//            using (DatabaseContext context = Util.CreateContext())
//            {
//                var feedback = (from f in context.BuyerFeedback
//                                where f.TradeGuid == id
//                                select f).First();

//                feedback.Status = model.Status;
//                feedback.Comment = model.Comment;
//                feedback.Rating = model.Rating;
//                feedback.LastModified = DateTime.UtcNow;

//                context.SaveChanges();
//            }
//        }


//    }
//}
