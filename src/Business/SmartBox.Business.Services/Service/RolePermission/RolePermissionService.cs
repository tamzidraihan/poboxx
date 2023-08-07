using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.RolePermission;
using SmartBox.Business.Core.Entities.UserRole;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.RolePermission;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Repository.RolePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.RolePermission
{
    public class RolePermissionService : BaseMessageService<RolePermissionService>, IRolePermissionService
    {
        private readonly IRolePermissionsRepository _rolePermissionsRepository;




        public RolePermissionService(IAppMessageService appMessageService, IMapper mapper, IRolePermissionsRepository rolePermissionsRepository) : base(appMessageService, mapper)
        {
            _rolePermissionsRepository = rolePermissionsRepository;

        }



        async Task<ResponseValidityModel> ValidateRole(MockRolePermissionModel rolePermissionModel)
        {
            var model = new ResponseValidityModel();
            bool isExisting = false;
            var rolePermissions = await _rolePermissionsRepository.GetRolePermissions(rolePermissionModel.RoleId);

            if (rolePermissions.Count > 0)
            {
                for (int i = 0; i < rolePermissions.Count; i++)
                {
                    if (rolePermissions[i].RoleId == rolePermissionModel.RoleId && rolePermissions[i].PermissionId == rolePermissionModel.PermissionId)
                    {
                        isExisting = true; break;
                    }
                }
            }

            if (isExisting)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.RoleAndPermissionId);
                return model;
            }

            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

            return model;
        }

        public async Task<ResponseValidityModel> Save(RolePermissionModel model)
        {
            var dataModel = new ResponseValidityModel();
            //bool isExisting = false;
            var rolePermissions = await _rolePermissionsRepository.GetRolePermissions(model.RoleId);


            /*foreach (var item in model.PermissionId)
            {
                foreach (var rolePermission in rolePermissions)
                {
                    if (item == rolePermission.PermissionId)
                    {
                        isExisting = true;
                        break;
                    }

                }

            }

            if (isExisting)
            {
                foreach (var rolePermission in rolePermissions)
                {
                    await _rolePermissionsRepository.DeleteRolePermission(rolePermission.RolePermissionId);
                }
            }*/

            foreach (var item in model.PermissionId)
            {
                MockRolePermissionModel mockRolePermission = new MockRolePermissionModel
                {
                    RolePermissionId = model.RolePermissionId,
                    PermissionId = item,
                    RoleId = model.RoleId,

                };

                dataModel = await ValidateRole(mockRolePermission);


                if (dataModel.MessageReturnNumber == 0)
                {
                    var entity = Mapper.Map<MockRolePermissionModel, RolePermissionEntity>(mockRolePermission);
                    var ret = await _rolePermissionsRepository.Save(entity);
                    dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();


                }
                else
                {
                    return dataModel;
                }
            }


            return dataModel;
        }

        public async Task<List<RolePermissionDetailModel>> GetRolePermissions(int? roleId = null)
        {
            var dbModel = await _rolePermissionsRepository.GetRolePermissions(roleId);
            if (dbModel != null)
            {
                var model = Mapper.Map<List<RolePermissionDetailModel>>(dbModel);
                return model;
            }

            return new List<RolePermissionDetailModel>();
        }

        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _rolePermissionsRepository.DeleteRolePermission(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<RolePermissionViewModel> GetById(int Id)
        {
            var dbModel = await _rolePermissionsRepository.GetById(Id);
            if (dbModel == null)
                return new RolePermissionViewModel();

            var model = Mapper.Map<RolePermissionViewModel>(dbModel);

            return model;
        }


    }
}
