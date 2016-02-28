using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class CommentView
    {
        public Guid Guid { get; set; }
        public Guid CreatedBy { get; set; } //AccountGuid
        public DateTime Created { get; set; }
        public string CreatedByUsername { get; set; }
        public string CreatedByFacebookUserId { get; set; }
        public string Role { get; set; } //Seller
        public string Body { get; set; }

    }
}