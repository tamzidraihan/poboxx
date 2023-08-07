using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Dashboard;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Location;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.Cabinet;
using SmartBox.Infrastructure.Data.Repository.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.Dashboard
{
    public class DashboardService : BaseMessageService<DashboardService>, IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
         
        public DashboardService(IAppMessageService appMessageService, IMapper mapper, IDashboardRepository dashboardRepository) : base(appMessageService, mapper)
        {
            _dashboardRepository = dashboardRepository; 
            
        }

        public DashboardViewModel GetDashboardData(int? companyid = null)
        {
            var data = _dashboardRepository.GetDashboardData(companyid);

            return data;
        }
    }
}
