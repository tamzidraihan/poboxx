using SmartBox.Business.Core.Models.Report;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Report
{
    public interface IReportService
    {
        Task<ResponseValidityModel> SaveCleanlinessReport(CleanlinessReportModel model);
        Task<CleanlinessReportParentViewModel> GetCleanlinessReport(int Month, int Year, int PageNumber, int PageSize, int? CompanyId, int? Status);
        Task<CleanlinessReportViewModel> CheckIsExist(int CompanyId,int Month,int Year);
        Task<ResponseValidityModel> DeleteCleanlinessReport(int id);
        Task MaintenanceReportReminderNotification();
        Task MaintenanceReportOverdueNotification();
    }
}
