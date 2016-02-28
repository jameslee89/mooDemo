using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class ViewCount
    {
        [Key]
        public Guid Guid { get; set; }
        public int Count { get; set; }
    }
}
