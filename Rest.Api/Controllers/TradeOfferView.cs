using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class TradeOfferView
    {
        public Guid Guid { get; set; }
        public string TradeCode { get; set; }
        public Guid FromAccountGuid { get; set; }
        public string FromUserName { get; set; }
        public Guid ToAccountGuid { get; set; }
        public string ToUserName { get; set; }
        public Guid OrderGuid { get; set; }
        public Guid ProductColourGuid { get; set; }
        public string ProductColourName { get; set; }
        public string ProductCondition { get; set; }
        public string ProductConditionName { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public Guid SkuGuid { get; set; }
        public string SkuFullName { get; set; }
        public string SkuImageUrl { get; set; }
        public string Status { get; set; } //Pending, Accepted, Expired, Cancelled, 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public string CancelReason { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime Deadline { get; set; }
    }
}