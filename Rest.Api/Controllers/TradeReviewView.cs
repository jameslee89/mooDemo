using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class TradeReviewView
    {
        public Guid Guid { get; set; }
        public Guid TradeGuid { get; set; }
        public string TradeCode { get; set; }
        public DateTime TradeCreated { get; set; }
        public string SkuUrl { get; set; }
        public string SkuImageUrl { get; set; }
        public string SkuFullName { get; set; }
        public string UserRole { get; set; }
        public bool UserFeedbackReceived { get; set; }
        public Guid BuyerGuid { get; set; } //accountOrStoreGuid
        public string BuyerName { get; set; }
        public string BuyerUserName { get; set; }
        public int BuyerRating { get; set; } //1 is the worse, 5 is the best, 0 is incomplete
        public string BuyerComment { get; set; }
        public Guid SellerGuid { get; set; } //accountOrStoreGuid
        public string SellerUserName { get; set; }
        public int SellerRating { get; set; } //1 is the worse, 5 is the best, 0 is incomplete
        public string SellerComment { get; set; }
    }
}