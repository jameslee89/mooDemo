using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class Account
    {
        [Key]
        public Guid Guid { get; set; }
        public string Username { get; set; } //must be unique
        public string FacebookUserId { get; set; }
        public string Email { get; set; } //optional
        public string Phone { get; set; } //must have
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public string Roles { get; set; }
        public string LanguageCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime Created { get; set; }
    }
}
