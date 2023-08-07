using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.User
{
    public interface IUserTokenRepository : IGenericRepositoryBase<UserTokenEntity>
    {
        Task<List<UserTokenEntity>> GetUserTokenByUserId(int userId, string deviceDecription = "", DeviceType? deviceType = null);
        Task<int> Save(UserTokenEntity model);
        Task<int> Delete(int id);
    }
}
