using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class AccountRating
    {
        [Key]
        public Guid AccountGuid { get; set; }
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
