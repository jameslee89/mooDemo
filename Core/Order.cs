using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class Order
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid SkuGuid { get; set; }
        public Guid CreatedByAccountGuid { get; set; }
        public Guid ProductColourGuid { get; set; }
        public int MinimumOrder { get; set; } //minimum order quantity for this order
        public int Quantity { get; set; }
        public int QuantityInitial { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; } //Buy or Sell
        public string Description { get; set; }
        public string ProductCondition { get; set; } //New, Used, Broken
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Location { get; set; }
        public string ImageGuids { get; set; } //jsonarray of imageGuid
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsExclusive { get; set; } //exclusive to mooketplace
    }
}
