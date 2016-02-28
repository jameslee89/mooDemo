using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class AccountSummaryController : ApiController
    {
        // GET api/<controller>/5
        public AccountSummary Get(Guid id)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var rating = (from r in context.AccountRatings
                              where r.AccountGuid == id
                              select r).FirstOrDefault();

                var account = (from a in context.Accounts
                               where a.Guid == id
                               select a).FirstOrDefault();

                if (account == null) return null;

                return Convert(account, rating);
            }
        }

        static AccountSummary Convert(Account account, AccountRating rating)
        {
            AccountSummary summary = new AccountSummary
            {
                AccountGuid = account.Guid,
                Username = account.Username,
                Created = account.Created
            };

            if (rating != null)
            {
                summary.BuyerRating = rating.BuyerRating;
                summary.BuyerRatingCount = rating.BuyerRatingCount;
                summary.LastModified = DateTime.SpecifyKind(rating.LastModified, DateTimeKind.Utc);
                summary.Rating = rating.Rating;
                summary.RatingCount = rating.RatingCount;
                summary.SellerRating = rating.SellerRating;
                summary.SellerRatingCount = rating.SellerRatingCount;
                summary.TotalSellTrades = rating.TotalSellTrades;
                summary.TotalBuyTrades = rating.TotalBuyTrades;
                summary.TotalTrades = rating.TotalTrades;
            }

            return summary;
        }

    }
}