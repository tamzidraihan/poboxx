using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Role;
using SmartBox.Business.Core.Entities.UserRole;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.Roles;
using SmartBox.Business.Core.Models.UserRole;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.Role;
using SmartBox.Infrastructure.Data.Repository.UserRole;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.UserRole
{
    public class UserRoleService : BaseMessageService<UserRoleService>, IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        
       

        
        public UserRoleService(IAppMessageService appMessageService, IMapper mapper, IUserRoleRepository userRoleRepository) : base(appMessageService, mapper)
        {
            _userRoleRepository = userRoleRepository;
             
        }

        async Task<ResponseValidityModel> ValidateUserRole(MockUserRole mockUserRole)
        {
            var model = new ResponseValidityModel();
            bool isExisting = false;
            var userRoles = await _userRoleRepository.GetUserRoles(mockUserRole.UserId);

            if (userRoles.Count > 0)
            {
                for (int i = 0; i < userRoles.Count; i++)
                {

                    if(userRoles[i].UserId == mockUserRole.UserId && userRoles[i].RoleId == mockUserRole.RoleId)
                    {
                        isExisting = true; break;
                    }  
                }
            }
            if (isExisting)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.RoleAndUserId);
                return model;
            }
 
            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

            return model;
        }

        public async Task<ResponseValidityModel> Save(UserRoleModel model)
        {
            var dataModel = new ResponseValidityModel();
   
            var userRoles = await _userRoleRepository.GetUserRoles(model.UserId);

            /*
            foreach (var item in model.RoleId)
            {
                foreach (var userRole in userRoles)
                {
                    if(item == userRole.RoleId)
                    {
                        isExisting = true;
                        break;
                    }

                }

            }

            if (isExisting)
            {
                foreach(var userRole in userRoles)
                {
                   await _userRoleRepository.DeleteUserRole(userRole.UserRoleId);
                }
            }
            */

            foreach (var item in model.RoleId)
            {
                MockUserRole mockUserRole = new MockUserRole
                { 
                    UserRoleId=model.UserRoleId,
                    RoleId=item,
                    UserId=model.UserId,
                  
                };

                dataModel = await ValidateUserRole(mockUserRole);

                if (dataModel.MessageReturnNumber == 0)
                {
                    var entity = Mapper.Map<MockUserRole, UserRoleEntity>(mockUserRole);
                    var ret = await _userRoleRepository.Save(entity);
                    dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
                   

                }
                else
                {
                    return dataModel;
                }
            }
           

            return dataModel;
        }
         

        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _userRoleRepository.DeleteUserRole(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<List<UserRoleViewModel>> GetAll(int userId)
        {
            var dbModel = await _userRoleRepository.GetUserRoles(userId);
            if (dbModel != null)
            {
                var model = Mapper.Map<List<UserRoleViewModel>>(dbModel);
                return model;
            }

            return new List<UserRoleViewModel>();
        }

        public async Task<UserRoleViewModel> GetById(int Id)
        {
            var dbModel = await _userRoleRepository.GetById(Id);
            if (dbModel == null)
                return new UserRoleViewModel();

            var model = Mapper.Map<UserRoleViewModel> (dbModel);

            return model;
        }

       
    }
}
