using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class OrderRequest
    {
        public Guid SkuGuid { get; set; }
        public Guid ProductColourGuid { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; } //Buy or Sell
        public string Location { get; set; }
        public string ProductCondition { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int MinimumOrder { get; set; }
        public int DurationDays { get; set; }
        public string[] Images { get; set; } //images in base64
    }
}