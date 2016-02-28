using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class TradeRequest
    {
        public Guid OrderGuid { get; set; }
        public int Quantity { get; set; }
    }
}