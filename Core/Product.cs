using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class Product
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid ManufacturerGuid { get; set; }
        public string NameTranslations { get; set; }
        public string DescriptionTranslations { get; set; }
        public string ManufacturerLink { get; set; }
    }
}
