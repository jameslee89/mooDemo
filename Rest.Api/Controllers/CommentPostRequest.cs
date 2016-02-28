using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class CommentPostRequest
    {
        public Guid OrderGuid { get; set; } //if filled nothing else is needed
        public Guid ReplyTo { get; set; }
        public string Body { get; set; }
    }
}