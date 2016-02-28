using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class Notification
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid RecipientGuid { get; set; } //AccountGuid
        public Guid SenderGuid { get; set; } //AccountGuid
        public string Type { get; set; } //OrderComment
        public string Href { get; set; }
        public bool IsRead { get; set; }
        public bool IsHidden { get; set; }
        public DateTime Created { get; set; }
    }
}
