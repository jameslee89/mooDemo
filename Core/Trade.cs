using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class Trade
    {
        [Key]
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
        public Guid SkuGuid { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } //Received / Ready For Pickup / Paid / Delivered / Cancelled
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
