using SmartBox.Business.Core.Entities.Pricing;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Pricing
{
    public interface IPriceAndChargingRepository : IGenericRepositoryBase<PriceAndChargingEntity>
    {
        Task<List<PriceAndChargingEntity>> Get(int? lockerTypeId = null, int? cabinetLocationId = null);
        Task<int> Save(PriceAndChargingEntity model);
        Task<int> Delete(int id);

    }
}
