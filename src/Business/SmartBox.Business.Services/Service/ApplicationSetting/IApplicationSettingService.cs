using SmartBox.Business.Core.Models.ApplicationSetting;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.ApplicationSetting
{
    public interface IApplicationSettingService
    {
        public Task<ApplicationSettingModel> GetApplicationSetting(short applicationSettingId = 1);
        Task<ResponseValidityModel> Save(ApplicationSettingModel param);
    }
}
