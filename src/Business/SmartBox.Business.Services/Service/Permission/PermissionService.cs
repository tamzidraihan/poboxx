using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Permission;
using SmartBox.Business.Core.Models.Permission;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.Permission
{
    public class PermissionService : BaseMessageService<PermissionService>, IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
         
        public PermissionService(IAppMessageService appMessageService, IMapper mapper, IPermissionRepository permissionRepository) : base(appMessageService, mapper)
        {
            _permissionRepository = permissionRepository;

        }

        async Task<ResponseValidityModel> ValidatePermission(PermissionModel permissionModel)
        {
            var model = new ResponseValidityModel();
            var isExisting = await _permissionRepository.GetByName(permissionModel.Name);

            if (isExisting != null)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.PermissionName);
                return model;
            }

            if (!permissionModel.Name.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.PermissionModel.PermissionName);
            }


            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

            return model;
        }

        public async Task<ResponseValidityModel> Save(PermissionModel model)
        {
            var dataModel = await ValidatePermission(model);


            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<PermissionModel, PermissionEntity>(model);
                var ret = await _permissionRepository.Save(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }


        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _permissionRepository.DeletePermission(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<List<PermissionViewModel>> GetAll()
        {
            var dbModel = await _permissionRepository.GetPermissions();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<PermissionViewModel>>(dbModel);
                return model;
            }

            return new List<PermissionViewModel>();
        }

        public async Task<PermissionViewModel> GetById(int Id)
        {
            var dbModel = await _permissionRepository.GetById(Id);
            if (dbModel == null)
                return new PermissionViewModel();

            var model = Mapper.Map<PermissionViewModel>(dbModel);

            return model;
        }

        public async Task<PermissionViewModel> GetByName(string Name)
        {
            var dbModel = await _permissionRepository.GetByName(Name);
            if (dbModel == null)
                return new PermissionViewModel();

            var model = Mapper.Map<PermissionViewModel>(dbModel);

            return model;
        }
    }
}
