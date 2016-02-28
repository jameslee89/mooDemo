using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class NewProductOrderRequest
    {
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public int Duration { get; set; }
    }
}