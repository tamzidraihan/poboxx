using SmartBox.Business.Core.Entities.RolePermission;
using SmartBox.Business.Core.Models.RolePermission;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.RolePermission
{
    public interface IRolePermissionsRepository : IGenericRepositoryBase<RolePermissionEntity>
    {
        Task<int> Save(RolePermissionEntity rolePermission);
        Task<RolePermissionEntity> GetById(int Id);

        Task<List<RolePermissionEntity>> GetRolePermissions(int? roleId = null);

        Task<int> DeleteRolePermission(int RolePermissionId);
    }
}
