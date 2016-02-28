using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class Comment
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid OrderGuid { get; set; }
        public DateTime Created { get; set; }
        public Guid CreatedBy { get; set; } //AccountGuid
        public Guid ReplyTo { get; set; } //CommentGuid
        public string Body { get; set; }
        public bool IsDeleted { get; set; }
    }
}
