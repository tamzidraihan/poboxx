using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.ApplicationSetting;
using SmartBox.Business.Core.Models.ApplicationSetting;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Infrastructure.Data.Repository.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.ApplicationSetting
{
    public class ApplicationSettingService : BaseMessageService<ApplicationSettingService>, IApplicationSettingService
    {
        private readonly IApplicationSettingRepository _applicationSettingRepository;
        public ApplicationSettingService(IAppMessageService appMessageService, IMapper mapper, IApplicationSettingRepository applicationSettingRepository) : base(appMessageService, mapper)
        {
            _applicationSettingRepository = applicationSettingRepository;
        }


        public async Task<ApplicationSettingModel> GetApplicationSetting(short applicationSettingId = 1)
        {
            var dbModel = await _applicationSettingRepository.GetApplicationSetting(applicationSettingId);
            var model = new ApplicationSettingModel();

            if (dbModel != null)
            {
                model = Mapper.Map<ApplicationSettingModel>(dbModel);
            }

            return model;

        }
        public async Task<ResponseValidityModel> Save(ApplicationSettingModel param)
        {
            if (param.ApplicationSettingsId < 1) return new ResponseValidityModel { MessageReturnNumber = 1, Message = "application setting Id is required!" };
            var entity = Mapper.Map<ApplicationSettingModel, ApplicationSettingEntity>(param);
            var ret = await _applicationSettingRepository.Save(entity);
            return AppMessageService.SetMessage(ret).MappedResponseValidityModel();

        }

    }
}
