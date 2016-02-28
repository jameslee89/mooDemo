using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class TradeOfferCancelRequest
    {
        public string Reason { get; set; }
        public string ReasonType { get; set; } //Out of stock, Other
    }
}