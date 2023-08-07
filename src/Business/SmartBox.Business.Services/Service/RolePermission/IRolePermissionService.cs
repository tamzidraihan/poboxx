using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.RolePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.RolePermission
{
    public interface IRolePermissionService
    {
        Task<ResponseValidityModel> Save(RolePermissionModel model);
        Task<List<RolePermissionDetailModel>> GetRolePermissions(int? roleId = null);
        Task<RolePermissionViewModel> GetById(int Id);
        Task<ResponseValidityModel> Delete(int Id);
    }
}
