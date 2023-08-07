using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.CompanyUser
{
    public class CompanyUserEntity
    {
        public int CompanyUserId { get; set; }
        public int CompanyId { get; set; }
        public string UserKeyId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActivated { get; set; }
        public bool IsSystemGenerated { get; set; }
    }
}
