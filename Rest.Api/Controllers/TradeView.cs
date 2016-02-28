using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class TradeView
    {
        public Guid Guid { get; set; }
        public string TradeCode { get; set; }
        public Guid BuyerGuid { get; set; }
        public string BuyerUserName { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public string BuyerLanguageCode { get; set; }
        public Guid SellerGuid { get; set; }
        public string SellerUserName { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string SellerLanguageCode { get; set; }
        public Guid OrderGuid { get; set; }
        public Guid ProductColourGuid { get; set; }
        public string ProductColourName { get; set; }
        public string ProductCondition { get; set; }
        public string ProductConditionName { get; set; }
        public Guid SkuGuid { get; set; }
        public string SkuFullName { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}