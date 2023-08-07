using SmartBox.Business.Core.Entities.Role;
using SmartBox.Business.Core.Entities.UserRole;
using SmartBox.Infrastructure.Data.Repository.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.UserRole
{
    public interface IUserRoleRepository : IGenericRepositoryBase<UserRoleEntity>
    {
        

        Task<int> Save(UserRoleEntity userRole);
        Task<UserRoleEntity> GetById(int Id);
    
        Task<List<UserRoleEntity>> GetUserRoles(int userId);

        Task<int> DeleteUserRole(int userRoleId);

    }
}
