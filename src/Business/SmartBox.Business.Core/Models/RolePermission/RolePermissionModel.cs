using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.RolePermission
{
    public class RolePermissionModel
    {
        public int RolePermissionId { get; set; }
        public List<int> PermissionId { get; set; }
        public int RoleId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
