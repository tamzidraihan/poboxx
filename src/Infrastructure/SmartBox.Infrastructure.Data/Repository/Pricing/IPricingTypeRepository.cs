using SmartBox.Business.Core.Entities.Pricing;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Pricing
{
    public interface IPricingTypeRepository : IGenericRepositoryBase<PricingTypeEntity>
    {
        Task<List<PricingTypeEntity>> Get();
        Task<int> Save(PricingTypeEntity model);
        Task<int> Delete(int id);
    }
}
