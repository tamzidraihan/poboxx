using SmartBox.Business.Core.Entities.Company;
using SmartBox.Business.Core.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Company
{
    public interface ICompanyCabinetRepository
    {
        Task<List<CompanyCabinetEntity>> Get(int? companyId = null, int? cabinetId = null, bool? unAssignedOnly = null);
        Task SaveBulkRecords(List<int> cabinetIds, int companyId);
        Task<int> Save(CompanyCabinetEntity model);
        Task<int> AssignCompanyCabinets(AssignCompanyCabinetModel model);
        Task<int> UnassignCompanyCabinets(AssignCompanyCabinetModel model);
    }
}
