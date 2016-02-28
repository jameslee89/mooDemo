using LinkenLabs.Market.Core;
using Microsoft.Ccr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Core
{
    public class AccountRatingGenerator
    {
        public static Port<EmptyValue> RunPort = new Port<EmptyValue>();

        public static void Execute(EmptyValue request)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                DateTime lastUpdate = DateTime.MinValue;

                if (context.AccountRatings.Count() > 0)
                {
                    lastUpdate = (from r in context.AccountRatings
                                  select r.LastModified).Max();
                }

                //get all ratings made since the last update
                var newRatings = (from r in context.TradeReviews
                                  where r.LastModified > lastUpdate
                                  select r).ToList();

                List<Guid> list = new List<Guid>();

                foreach (var item in newRatings)
                {
                    if (!list.Contains(item.BuyerGuid))
                    {
                        list.Add(item.BuyerGuid);
                    }

                    if (!list.Contains(item.SellerGuid))
                    {
                        list.Add(item.SellerGuid);
                    }
                }

                List<AccountRating> ratings = list.ConvertAll(CalculateRating);

                ratings.ForEach((rating) =>
                {
                    var existingRating = (from a in context.AccountRatings
                                          where a.AccountGuid == rating.AccountGuid
                                          select a).FirstOrDefault();

                    if (existingRating == null)
                    {
                        context.AccountRatings.Add(rating);
                    }
                    else
                    {
                        existingRating.BuyerRating = rating.BuyerRating;
                        existingRating.BuyerRatingCount = rating.BuyerRatingCount;
                        existingRating.LastModified = rating.LastModified;
                        existingRating.Rating = rating.Rating;
                        existingRating.RatingCount = rating.RatingCount;
                        existingRating.SellerRating = rating.SellerRating;
                        existingRating.SellerRatingCount = rating.SellerRatingCount;
                    }
                    context.SaveChanges();
                });


            }
        }

        static AccountRating CalculateRating(Guid accountGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                AccountRating rating = new AccountRating
                {
                    AccountGuid = accountGuid
                };

                var relatedReviews = (from r in context.TradeReviews
                                      where r.SellerGuid == accountGuid || r.BuyerGuid == accountGuid
                                      select r).ToList();

                //get sellerRating
                var sellerReviews = (from r in relatedReviews
                                     where r.SellerGuid == accountGuid && r.BuyerRating > 0
                                     select r).ToList();

                rating.SellerRating = sellerReviews.Count == 0 ? 0 : Convert.ToDecimal(sellerReviews.Average(t => t.BuyerRating));
                rating.SellerRatingCount = sellerReviews.Count;

                var buyerReviews = (from r in relatedReviews
                                    where r.BuyerGuid == accountGuid && r.SellerRating > 0
                                    select r).ToList();

                rating.BuyerRating = buyerReviews.Count == 0 ? 0 : Convert.ToDecimal(buyerReviews.Average(t => t.SellerRating));
                rating.BuyerRatingCount = buyerReviews.Count;

                var totalSellerScore = sellerReviews.Sum(t => t.BuyerRating);
                var totalBuyerScore = buyerReviews.Sum(t => t.SellerRating);

                var totalCount = sellerReviews.Count + buyerReviews.Count;

                rating.Rating = totalCount == 0 ? 0 : (totalSellerScore + totalBuyerScore) / totalCount;
                rating.RatingCount = totalCount;
                rating.LastModified = relatedReviews.Max(t => t.LastModified);
                
                var totalBuyTrades = (from t in context.Trades
                              where t.BuyerGuid == accountGuid
                              select t).Count();

                var totalSellTrades = (from t in context.Trades
                                    where t.SellerGuid == accountGuid
                                    select t).Count();

                rating.TotalSellTrades = totalSellTrades;
                rating.TotalBuyTrades = totalBuyTrades;
                rating.TotalTrades = totalSellTrades + totalBuyTrades;

                return rating;
            }
        }
    }
}