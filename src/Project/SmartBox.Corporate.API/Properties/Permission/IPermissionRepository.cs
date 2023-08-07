using SmartBox.Business.Core.Entities.Permission;
using SmartBox.Business.Core.Entities.Role;
using SmartBox.Business.Core.Entities.UserRole;
using SmartBox.Infrastructure.Data.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Permission
{
    public interface IRolePermissionRepository : IGenericRepositoryBase<PermissionEntity>
    {
        Task<int> SetPermissionEntity(PermissionEntity permissioEntity);
        Task<List<PermissionEntity>> GetPermission();

        Task<int> DeletePermission(int permissionId);

    }
}
