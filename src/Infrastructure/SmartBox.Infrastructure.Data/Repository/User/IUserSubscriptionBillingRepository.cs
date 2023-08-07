using SmartBox.Business.Core.Entities.User;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.User
{
    public interface IUserSubscriptionBillingRepository : IGenericRepositoryBase<UserSubscriptionBillingEntity>
    {
        Task<List<UserSubscriptionBillingEntity>> GetUserSubscriptionBillingBySubscriptionId(int userSubscriptionId);
        Task<int> Save(UserSubscriptionBillingEntity model);
        Task<int> Delete(int id);
    }
}
