using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class AccountSummary
    {
        public Guid AccountGuid { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public decimal Rating { get; set; }
        public int RatingCount { get; set; }
        public decimal SellerRating { get; set; }
        public int SellerRatingCount { get; set; }
        public decimal BuyerRating { get; set; }
        public int BuyerRatingCount { get; set; }
        public DateTime LastModified { get; set; }
        public int TotalSellTrades { get; set; }
        public int TotalBuyTrades { get; set; }
        public int TotalTrades { get; set; }
    }
}