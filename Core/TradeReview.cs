using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class TradeReview
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid TradeGuid { get; set; }
        public DateTime TradeCreated { get; set; }
        public Guid BuyerGuid { get; set; } //accountOrStoreGuid
        public int BuyerRating { get; set; } //this is the rating FROM THE BUYER
        public string BuyerComment { get; set; } //this is the rating comment FROM THE BUYER
        public Guid SellerGuid { get; set; } //accountOrStoreGuid
        public int SellerRating { get; set; } //this is the rating given FROM THE SELLER
        public string SellerComment { get; set; } //this is the rating comment FROM THE SELLER
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
