using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class OrderView : OrderInfo
    {
        public string SkuName { get; set; }
        public string SkuFullName { get; set; }
        public string AccountUserName { get; set; }
        public string AccountPhone { get; set; }
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; }
        public Guid ManufacturerGuid { get; set; }
        public string ManufacturerName { get; set; }
        public Guid CategoryGuid { get; set; }
        public string CategoryName { get; set; }
        public string LocationName { get; set; } //Location in formatted in selected language
        public string ProductConditionName { get; set; }
        public string ProductColourValue { get; set; }
        public string ProductColourName { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Location { get; set; }
        public string ImageSrc { get; set; }
        public string Status { get; set; } //Active, //Sold out, //Expired, 
        public bool IsActive { get; set; }
        public string Url { get; set; }
        public int ViewCount { get; set; }
        public string[] ImageUrls { get; set; }
    }
}