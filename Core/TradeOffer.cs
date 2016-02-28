using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    //can later be used for counteroffers as well
    public class TradeOffer
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid OrderGuid { get; set; }
        public Guid FromAccountGuid { get; set; }
        public Guid ToAccountGuid { get; set; }
        public string TradeCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime Deadline { get; set; } //24 hours later
        public string CancelReason { get; set; }
    }
}
