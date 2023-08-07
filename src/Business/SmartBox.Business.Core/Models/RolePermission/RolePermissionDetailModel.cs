using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.RolePermission
{
    public class RolePermissionDetailModel
    {
        public int RolePermissionId { get; set; }
        public int PermissionId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string PermissionName { get; set; }
    }
}
