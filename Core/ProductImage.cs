using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class ProductImage
    {
        [Key]
        public Guid ProductGuid { get; set; }
        public string ImagesData { get; set; }
    }
}
