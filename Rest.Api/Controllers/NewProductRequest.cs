using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class NewProductRequest
    {
        public string Manufacturer { get; set; }
        public string Product { get; set; }
        public string Model { get; set; }
    }
}