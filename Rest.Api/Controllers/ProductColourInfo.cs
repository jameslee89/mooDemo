using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ProductColourInfo
    {
        public Guid Guid { get; set; }
        public Guid ProductGuid { get; set; }
        public TranslationInfo[] NameTranslations { get; set; }
        public string Value { get; set; } //#RRGGBB
    }
}