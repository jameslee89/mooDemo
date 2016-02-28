using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class ProductCategory
    {
        [Column(Order = 0), Key]
        public Guid ProductGuid { get; set; }
        [Column(Order = 1), Key]
        public Guid CategoryGuid { get; set; }
    }
}
