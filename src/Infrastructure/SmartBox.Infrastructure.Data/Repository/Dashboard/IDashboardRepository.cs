using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Models.Dashboard;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Dashboard
{
    public interface IDashboardRepository : IGenericRepositoryBase<DashboardViewModel>
    {
        DashboardViewModel GetDashboardData(int? companyid = null);


    }
}
