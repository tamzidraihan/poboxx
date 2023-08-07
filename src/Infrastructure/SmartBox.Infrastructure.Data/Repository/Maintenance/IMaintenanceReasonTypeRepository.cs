using SmartBox.Business.Core.Entities.Maintenance;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Maintenance
{
    public interface IMaintenanceReasonTypeRepository : IGenericRepositoryBase<MaintenanceReasonTypeEntity>
    {
        Task<List<MaintenanceReasonTypeEntity>> Get();
        Task<int> Save(MaintenanceReasonTypeEntity model);
        Task<int> Delete(int id);
    }
}
