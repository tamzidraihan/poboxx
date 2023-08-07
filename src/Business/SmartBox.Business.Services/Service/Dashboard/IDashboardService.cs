using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Dashboard;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Dashboard
{
    public interface IDashboardService
    {
        DashboardViewModel GetDashboardData(int? companyid = null);
    }
}
