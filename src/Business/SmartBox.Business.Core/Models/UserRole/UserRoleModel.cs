using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.UserRole
{
    public class UserRoleModel
    {
        public int UserRoleId { get; set; }
        public List<int> RoleId { get; set; }
        public string Username { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
