using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class SkuInfo
    {
        public Guid Guid { get; set; }
        public Guid ProductGuid { get; set; }
        public TranslationInfo[] NameTranslations { get; set; }
    }
}