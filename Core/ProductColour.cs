using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class ProductColour
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid ProductGuid { get; set; }
        public string NameTranslations { get; set; }
        public string Value { get; set; } //#RRGGBB
        
    }
}
