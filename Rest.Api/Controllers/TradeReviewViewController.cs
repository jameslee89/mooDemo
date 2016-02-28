using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class TradeReviewViewController : ApiController
    {
        // GET api/<controller>
        [BasicAuthentication]
        [Route("~/v1/tradeReviewView")]
        public IEnumerable<TradeReviewView> GetList()
        {

            Account userAccount = AccountController.GetAccountByUsername(User.Identity.Name);

            using (DatabaseContext context = new DatabaseContext())
            {
                //get all reviews
                var reviews = (from r in context.TradeReviews
                               where r.SellerGuid == userAccount.Guid || r.BuyerGuid == userAccount.Guid
                               orderby r.TradeCreated ascending
                               select r).ToList();
                var convertFunc = new Converter<TradeReview, TradeReviewView>((TradeReview t) =>
                {
                    return Convert(t, userAccount.Guid);
                });
                var converted = reviews.ConvertAll(convertFunc);

                return converted;
            }
        }

        [BasicAuthentication]
        public TradeReviewView Get(Guid id)
        {
            AccountInfo account = new AccountController().Get();

            using (DatabaseContext context = new DatabaseContext())
            {
                //get all reviews
                var item = (from r in context.TradeReviews
                            where r.Guid == id
                            select r).FirstOrDefault();

                if (item == null)
                {
                    return null;
                }

                //if logged in account is not buyer or seller, reject
                if (account.Guid != item.BuyerGuid && account.Guid != item.SellerGuid)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                TradeReviewView converted = Convert(item, account.Guid);

                return converted;
            }
        }

        static TradeReviewView Convert(TradeReview review, Guid accountOrStoreGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Trade trade = (from t in context.Trades
                               where t.Guid == review.TradeGuid
                               select t).First();

                SkuView skuView = new SkuViewController().GetList(skus: trade.SkuGuid.ToString(), showInactive: true).ToList().First();

                Account buyer = (from a in context.Accounts
                                 where a.Guid == review.BuyerGuid
                                 select a).First();

                Account seller = (from a in context.Accounts
                                  where a.Guid == review.SellerGuid
                                  select a).First();
                var userRole = trade.BuyerGuid == accountOrStoreGuid ? "Buyer" : "Seller";
                var userFeedbackReceived = trade.BuyerGuid == accountOrStoreGuid ? review.BuyerRating > 0 : review.SellerRating > 0;
                return new TradeReviewView
                {
                    SkuFullName = skuView.FullName,
                    SkuUrl = skuView.Url,
                    SkuImageUrl = skuView.ImageSrc,
                    TradeCode = trade.TradeCode,
                    BuyerComment = review.BuyerComment,
                    BuyerGuid = review.BuyerGuid,
                    BuyerUserName = buyer.Username,
                    BuyerRating = review.BuyerRating,
                    Guid = review.Guid,
                    SellerComment = review.SellerComment,
                    SellerGuid = review.SellerGuid,
                    SellerUserName = seller.Username,
                    SellerRating = review.SellerRating,
                    TradeCreated = review.TradeCreated,
                    TradeGuid = review.TradeGuid,
                    UserRole = userRole,
                    UserFeedbackReceived = userFeedbackReceived
                };
            }
        }

    }
}