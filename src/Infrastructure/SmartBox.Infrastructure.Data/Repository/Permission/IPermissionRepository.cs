using SmartBox.Business.Core.Entities.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Permission
{
    public interface IPermissionRepository
    {
        Task<int> Save(PermissionEntity permission);
        Task<PermissionEntity> GetById(int Id);
        Task<PermissionEntity> GetByName(string Name);
        Task<List<PermissionEntity>> GetPermissions();

        Task<int> DeletePermission(int Id);
    }
}
