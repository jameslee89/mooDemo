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
    public class TradeReviewController : ApiController
    {
        // PUT api/<controller>/5
        [BasicAuthentication]
        public void Put(Guid id, TradeReviewInfo value)
        {
            AccountInfo account = new AccountController().Get();

            using (DatabaseContext context = new DatabaseContext())
            {
                TradeReview review = (from t in context.TradeReviews
                                      where t.Guid == id
                                      select t).First();

                if (review.BuyerGuid != account.Guid && review.SellerGuid != account.Guid)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }

                //this feedback is from the BUYER
                if (review.BuyerGuid == account.Guid)
                {
                    review.BuyerRating = value.Rating;
                    review.BuyerComment = value.Comment;
                }
                else if (review.SellerGuid == account.Guid)
                {
                    review.SellerRating = value.Rating;
                    review.SellerComment = value.Comment;
                }

                review.LastModified = DateTime.UtcNow;
                context.SaveChanges();                
            }
        }

    }
}