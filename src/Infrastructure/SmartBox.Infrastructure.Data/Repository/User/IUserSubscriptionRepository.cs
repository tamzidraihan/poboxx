using SmartBox.Business.Core.Entities.User;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.User
{
    public interface IUserSubscriptionRepository : IGenericRepositoryBase<UserSubscriptionEntity>
    {
        Task<List<UserSubscriptionEntity>> GetUserSubscription(string userKeyId, int? lockerDetailId = null, DateTime? storagePeriodEndDate = null);
        Task<Tuple<int, int>> Save(UserSubscriptionEntity model, string loginUserKeyId = null);
        Task<int> Delete(int id);
        Task<List<UserSubscriptionExpirationEntity>> GetUserSubscriptionExpiration();
        Task<UserSubscriptionEntity> GetById(int id);
    }
}
