using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class SkuView
    {
        public Guid Guid { get; set; }
        public Guid ProductGuid { get; set; }
        public Guid ManufacturerGuid { get; set; }
        public Guid CategoryGuid { get; set; }
        public string CategoryName { get; set; }
        public string ManufacturerName { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string SkuName { get; set; }
        public string FullName { get; set; }
        public bool IsExclusive { get; set; }
        public int OrdersCount { get; set; }
        public decimal PriceLow { get; set; }
        public string Currency { get; set; }
        public DateTime LastModified { get; set; } //for sitemap lastmod 
        public string Keywords { get; set; }
        public string ImageSrc { get; set; }
        public string Url { get; set; }
        public string ManufacturerLink { get; set; }
        public int SoldCount { get; set; }
        public int ViewCount { get; set; }

    }
}