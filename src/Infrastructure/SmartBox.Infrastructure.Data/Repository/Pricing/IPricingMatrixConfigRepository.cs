using SmartBox.Business.Core.Entities.Pricing;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Pricing
{
    public interface IPricingMatrixConfigRepository : IGenericRepositoryBase<PricingMatrixConfigEntity>
    {
        Task<List<PricingMatrixConfigEntity>> Get(int? PricingTypeId = null, int? selectedId = null, short? isActive = null);
        Task<int> Save(PricingMatrixConfigEntity model);
        Task<int> Delete(int id);
        Task<int> ActivateDeactivate(int Id, int? isActive);
    }
}
