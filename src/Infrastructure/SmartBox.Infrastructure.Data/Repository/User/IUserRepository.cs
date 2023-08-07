using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Core.Models.User;
using SmartBox.Infrastructure.Data.Repository.Base;

namespace SmartBox.Infrastructure.Data.Repository.User
{
    public interface IUserRepository : IGenericRepositoryBase<UserEntity>
    {
        Task<int> SetUserEntity(UserEntity userEntity);
        Task<int> UpdateUserEntity(UserEntity userEntity);
        Task<List<UserEntity>> GetAppUsers();
        Task<int> UpdateUserOTP(UserEntity userEntity);
        Task<int> UpdateUserMPIN(UserEntity userEntity);
        Task<int> ActivateUser(string userKeyId);
        Task<UserEntity> GetUserFromPhoneNo(string phoneNumber);
        Task<AdminUserEntity> GetAdminUser(string username, string email);
        Task<int> SetAdminUserEntity(AdminUserEntity adminUserEntity);
        Task<UserEntity> GetUserByUserKeyId(string userKeyId);
        Task<int> GetLastIdentity();
        Task<UserEntity> GetUserFromEmail(string email);
        Task<List<AdminUserEntity>> GetAdminUsers(string userName = null, string email = null);
        Task<int> UpdateUserPassword(int userId, string newHashPassword);
        Task<int> DeleteAdminUserEntity(int id);
        Task<int> DeleteAppUserEntity(string userKeyId);

        Task<int> CreateUserFavouriteCabinetLocations(string userKeyId, int cabinetLocartionId);
        Task<int> DeleteUserFavouriteCabinetLocations(int Id);
        Task<List<CabinetLocationEntity>> GetUserFavoritesCabinetLocations(string userKeyId);
        Task<List<UserFavouriteLocationModel>> GetUserFavoritesCabinetLocationsList(string userKeyId);


    }
}
