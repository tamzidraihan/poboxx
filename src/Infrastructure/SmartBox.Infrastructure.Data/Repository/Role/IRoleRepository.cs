using SmartBox.Business.Core.Entities.Role;
using SmartBox.Infrastructure.Data.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Role
{
    public interface IRoleRepository : IGenericRepositoryBase<RoleEntity>
    {
        Task<int> Save(RoleEntity role);
        Task<RoleEntity> GetById(int Id);
        Task<RoleEntity> GetByName(string roleName);
        Task<List<RoleEntity>> GetRoles();

        Task<int> DeleteRole(int roleId);

    }
}
