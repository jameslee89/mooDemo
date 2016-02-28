using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ProductColourView
    {
        public Guid Guid { get; set; }
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}