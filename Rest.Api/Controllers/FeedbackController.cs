using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class FeedbackController : ApiController
    {
        // GET api/<controller>
        [Route("~/v1/feedback/{id}")]
        public IEnumerable<FeedbackInfo> GetFeedbackReceived(Guid id)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                //get all reviews for this user
                var reviews = (from r in context.TradeReviews
                               where (r.SellerGuid == id && r.BuyerRating > 0)
                               || (r.BuyerGuid == id && r.SellerRating > 0)
                               orderby r.TradeCreated ascending
                               select r).ToList();

                var convertFunc = new Converter<TradeReview, FeedbackInfo>((TradeReview t) =>
                {
                    return ConvertFeedbackReceived(t, id);
                });

                var converted = reviews.ConvertAll(convertFunc);

                return converted;
            }
        }

        static FeedbackInfo ConvertFeedbackReceived(TradeReview review, Guid revieweeGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var reviewerGuid = review.BuyerGuid == revieweeGuid ? review.SellerGuid : review.BuyerGuid;
                var reviewerComment = review.BuyerGuid == revieweeGuid ? review.SellerComment : review.BuyerComment;
                var reviewerRating = review.BuyerGuid == revieweeGuid ? review.SellerRating : review.BuyerRating;
                var reviewer = (from a in context.Accounts
                                where a.Guid == reviewerGuid
                                select a).First();

                var reviewee = (from a in context.Accounts
                                where a.Guid == revieweeGuid
                                select a).First();


                return new FeedbackInfo
                {
                    ReviewerGuid = reviewerGuid,
                    ReviewerName = reviewer.Username,
                    RevieweeGuid = revieweeGuid,
                    RevieweeName = reviewee.Username,
                    Comment = reviewerComment,
                    Rating = reviewerRating,
                    Created = review.Created
                };
            }
        }

        // GET api/<controller>
        [Route("~/v1/feedbackGiven/{id}")]
        public IEnumerable<FeedbackInfo> GetFeedbackGiven(Guid id)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                //get all reviews given by this user
                var reviews = (from r in context.TradeReviews
                               where (r.SellerGuid == id && r.SellerRating > 0)
                               || (r.BuyerGuid == id && r.BuyerRating > 0)
                               orderby r.TradeCreated ascending
                               select r).ToList();

                var convertFunc = new Converter<TradeReview, FeedbackInfo>((TradeReview t) =>
                {
                    return ConvertFeedbackGiven(t, id);
                });

                var converted = reviews.ConvertAll(convertFunc);

                return converted;
            }
        }

        static FeedbackInfo ConvertFeedbackGiven(TradeReview review, Guid reviewerGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var revieweeGuid = review.BuyerGuid == reviewerGuid ? review.SellerGuid : review.BuyerGuid;
                var reviewerComment = review.BuyerGuid == reviewerGuid ? review.BuyerComment : review.SellerComment;
                var reviewerRating = review.BuyerGuid == reviewerGuid ? review.BuyerRating : review.SellerRating;
                var reviewer = (from a in context.Accounts
                                where a.Guid == reviewerGuid
                                select a).First();

                var reviewee = (from a in context.Accounts
                                where a.Guid == revieweeGuid
                                select a).First();

                return new FeedbackInfo
                {
                    ReviewerGuid = reviewerGuid,
                    ReviewerName = reviewer.Username,
                    RevieweeGuid = revieweeGuid,
                    RevieweeName = reviewee.Username,
                    Comment = reviewerComment,
                    Rating = reviewerRating,
                    Created = review.Created
                };
            }
        }
    }
}