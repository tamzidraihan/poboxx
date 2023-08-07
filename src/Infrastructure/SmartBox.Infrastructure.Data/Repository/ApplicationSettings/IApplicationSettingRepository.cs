using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Entities.ApplicationSetting;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Infrastructure.Data.Repository.Base;

namespace SmartBox.Infrastructure.Data.Repository.ApplicationSettings
{
    public interface IApplicationSettingRepository : IGenericRepositoryBase<ApplicationSettingEntity>
    {
        Task<ApplicationSettingEntity> GetApplicationSetting(short applicationSettingId = 1);
        Task<int> Save(ApplicationSettingEntity model);
    }
}
