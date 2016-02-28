using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class NotificationSmsDelivery
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid RecipientGuid { get; set; }
        public string Phone { get; set; }
        public string SenderId { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime Sent { get; set; }
    }
}
