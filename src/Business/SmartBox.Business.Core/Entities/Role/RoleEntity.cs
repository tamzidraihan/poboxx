using SmartBox.Business.Core.Entities.Base;
using System;

namespace SmartBox.Business.Core.Entities.Role
{
    public class RoleEntity : BaseEntity
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
