using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class SmsConfirmationCode
    {
        [Key]
        public Guid Guid { get; set; }
        public string MobileNumber { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime Created { get; set; }
    }
}
