using LinkenLabs.Market.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class ProductInfo
    {
        public Guid Guid { get; set; }
        public Guid ManufacturerGuid { get; set; }
        public TranslationInfo[] NameTranslations { get; set; }
        public TranslationInfo[] DescriptionTranslations { get; set; }
        public Guid CategoryGuid { get; set; }
        public string ManufacturerLink { get; set; }
        public string CategoryName { get; set; }
    }
}