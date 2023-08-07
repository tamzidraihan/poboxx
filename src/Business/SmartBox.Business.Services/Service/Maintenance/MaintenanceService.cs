using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Maintenance;
using SmartBox.Business.Core.Models.Announcement;
using SmartBox.Business.Core.Models.Maintenance;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Infrastructure.Data.Repository.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Maintenance
{
    public class MaintenanceService : BaseMessageService<MaintenanceService>, IMaintenanceService
    {
        private readonly IMaintenanceInspectionTestingRepository maintenanceInspectionTestingRepository;
        private readonly IMaintenanceReasonTypeRepository maintenanceReasonTypeRepository;
        public MaintenanceService(IAppMessageService appMessageService, IMapper mapper,
            IMaintenanceInspectionTestingRepository maintenanceInspectionTestingRepository,
            IMaintenanceReasonTypeRepository maintenanceReasonTypeRepository
            ) : base(appMessageService, mapper)
        {
            this.maintenanceInspectionTestingRepository = maintenanceInspectionTestingRepository;
            this.maintenanceReasonTypeRepository = maintenanceReasonTypeRepository;
        }
        public async Task<ResponseValidityModel> SaveMaintenanceInspectionTesting(MaintenanceInspectionTestingModel type)
        {
            var model = ValidateMaintenanceInspectionTesting(type);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<MaintenanceInspectionTestingModel, MaintenanceInspectionTestingEntity>(type);
                var ret = await maintenanceInspectionTestingRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidateMaintenanceInspectionTesting(MaintenanceInspectionTestingModel param)
        {
            var model = new ResponseValidityModel();
            if (param.TypeId < 1 || param.LockerTypeId < 1 ||
                param.CompanyId < 1 || param.CabinetId < 1 ||
                param.LockerDetailId < 1 || param.MaintenanceReasonTypeId < 1)
                model.MessageReturnNumber = 1;

            return model;
        }
        public async Task<List<MaintenanceInspectionTestingViewModel>> GetMaintenanceInspectionTesting(DateTime? fromDate, DateTime? toDate, int? companyId, int? cabinetLocationId)
        {
            var dbModel = await maintenanceInspectionTestingRepository.Get(fromDate, toDate, companyId, cabinetLocationId);
            var model = new List<MaintenanceInspectionTestingViewModel>();
            if (dbModel != null)
                model = Mapper.Map<List<MaintenanceInspectionTestingViewModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> DeleteMaintenanceInspectionTesting(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await maintenanceInspectionTestingRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
        public async Task<ResponseValidityModel> SaveMaintenanceReasonType(MaintenanceReasonTypeModel type)
        {
            var model = ValidateMaintenanceReasonType(type);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<MaintenanceReasonTypeModel, MaintenanceReasonTypeEntity>(type);
                var ret = await maintenanceReasonTypeRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidateMaintenanceReasonType(MaintenanceReasonTypeModel type)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(type.Name))
                model.MessageReturnNumber = 1;

            return model;
        }
        public async Task<List<MaintenanceReasonTypeModel>> GetMaintenanceReasonType()
        {
            var dbModel = await maintenanceReasonTypeRepository.Get();
            var model = new List<MaintenanceReasonTypeModel>();

            if (dbModel != null)
                model = Mapper.Map<List<MaintenanceReasonTypeModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> DeleteMaintenanceReasonType(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await maintenanceReasonTypeRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
    }
}
