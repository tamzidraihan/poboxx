using SmartBox.Business.Core.Entities.User;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.User
{
    public interface IChargesRepository : IGenericRepositoryBase<ChargesEntity>
    {
        Task<List<ChargesEntity>> GetChargesByUserKeyId(string userKeyId, string paymentRefNo = null);
        Task<int> Save(ChargesEntity model);
        Task<int> Delete(int id);
    }
}
