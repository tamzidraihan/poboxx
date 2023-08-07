using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Permission
{
    public class PermissionViewModel : PermissionModel
    {
        public bool IsDeleted { get; set; }
    }
}
