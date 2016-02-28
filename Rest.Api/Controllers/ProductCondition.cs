using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ProductCondition
    {
        public TranslationInfo[] Names { get; set; }
        public string Key { get; set; }
    }
}