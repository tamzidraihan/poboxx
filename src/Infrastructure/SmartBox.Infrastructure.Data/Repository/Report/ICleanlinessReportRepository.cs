using SmartBox.Business.Core.Entities.Report;
using SmartBox.Business.Core.Models.Report;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Report
{
    public interface ICleanlinessReportRepository : IGenericRepositoryBase<CleanlinessReportEntity>
    {
        Task<List<CleanlinessReportEntity>> Get(int month, int year, int PageNumber, int PageSize, int? companyId = null, int? Status = null);
     
        Task<CleanlinessReportEntity> CheckIsExist(int companyId,int Month,int Year);
        Task<int> Save(CleanlinessReportEntity model);
        Task<int> Delete(int id); 
        Task<List<UnsubmittedMaintenanceReportModel>> GetUnsubmittedMaintenanceReport(int month);
    }
}
