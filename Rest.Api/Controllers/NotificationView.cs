using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class NotificationView
    {
        public Guid Guid { get; set; }
        public Guid RecipientGuid { get; set; } //AccountGuid
        public Guid SenderGuid { get; set; } //AccountGuid
        public string SenderPictureUrl { get; set; }
        public string Type { get; set; } //OrderComment
        public bool IsRead { get; set; }
        public bool IsHidden { get; set; }
        public DateTime Created { get; set; }
        public string BodyHtml { get; set; }
        public string Url { get; set; }
    }
}
