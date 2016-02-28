using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class OrderInfo
    {
        public Guid Guid { get; set; }
        public Guid SkuGuid { get; set; }
        public Guid CreatedByAccountGuid { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; } //Buy or Sell
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public Guid ProductColourGuid { get; set; }
        public string ProductCondition { get; set; }
        public string Location { get; set; }
        public bool IsExclusive { get; set; }
        public int Quantity { get; set; }
        public int QuantityInitial { get; set; }
        public int MinimumOrder { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}