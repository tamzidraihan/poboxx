using SmartBox.Business.Core.Entities.Maintenance;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Maintenance
{
    public interface IMaintenanceInspectionTestingRepository : IGenericRepositoryBase<MaintenanceInspectionTestingEntity>
    {
        Task<List<MaintenanceInspectionTestingEntity>> Get(DateTime? fromDate, DateTime? toDate, int? companyId, int? cabinetLocationId);
        Task<int> Save(MaintenanceInspectionTestingEntity model);
        Task<int> Delete(int id);
    }
}
