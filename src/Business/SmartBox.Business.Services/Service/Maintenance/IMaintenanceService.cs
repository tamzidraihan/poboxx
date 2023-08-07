using SmartBox.Business.Core.Models.Maintenance;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Maintenance
{
    public interface IMaintenanceService
    {
        Task<ResponseValidityModel> SaveMaintenanceReasonType(MaintenanceReasonTypeModel type);
        Task<List<MaintenanceReasonTypeModel>> GetMaintenanceReasonType();
        Task<ResponseValidityModel> DeleteMaintenanceReasonType(int id);
        Task<ResponseValidityModel> SaveMaintenanceInspectionTesting(MaintenanceInspectionTestingModel type);
        Task<List<MaintenanceInspectionTestingViewModel>> GetMaintenanceInspectionTesting(DateTime? fromDate, DateTime? toDate, int? companyId, int? cabinetLocationId);
        Task<ResponseValidityModel> DeleteMaintenanceInspectionTesting(int id);
       
    }
}
