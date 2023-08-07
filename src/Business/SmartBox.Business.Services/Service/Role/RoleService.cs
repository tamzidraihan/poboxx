using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Role;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.Roles;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.Role;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.Role
{
    public class RoleService : BaseMessageService<RoleService>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
       

        
        public RoleService(IAppMessageService appMessageService, IMapper mapper,IRoleRepository roleRepository) : base(appMessageService, mapper)
        {
            _roleRepository = roleRepository;
          
        }

        async Task<ResponseValidityModel> ValidateRole(RoleModel roleModel)
        {
            var model = new ResponseValidityModel();
            var isExisting = await _roleRepository.GetByName(roleModel.RoleName);

            if (isExisting != null)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.RoleName);
                return model;
            }

            if (!roleModel.RoleName.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.RoleModel.RoleName);
            }


            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

            return model;
        }

        public async Task<ResponseValidityModel> Save(RoleModel model)
        {
            var dataModel = await ValidateRole(model);
            

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<RoleModel, RoleEntity>(model);
                var ret = await _roleRepository.Save(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }
         

        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _roleRepository.DeleteRole(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<List<RoleViewModel>> GetAll()
        {
            var dbModel = await _roleRepository.GetRoles();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<RoleViewModel>>(dbModel);
                return model;
            }

            return new List<RoleViewModel>();
        }

        public async Task<RoleViewModel> GetById(int Id)
        {
            var dbModel = await _roleRepository.GetById(Id);
            if (dbModel == null)
                return new RoleViewModel();

            var model = Mapper.Map<RoleViewModel> (dbModel);

            return model;
        }

        public async Task<RoleViewModel> GetByName(string Name)
        {
            var dbModel = await _roleRepository.GetByName(Name);
            if (dbModel == null)
                return new RoleViewModel();

            var model = Mapper.Map<RoleViewModel>(dbModel);

            return model;
        }
    }
}
