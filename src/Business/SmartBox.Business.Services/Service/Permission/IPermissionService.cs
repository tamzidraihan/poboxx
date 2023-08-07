using SmartBox.Business.Core.Models.Permission;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Permission
{
    public interface IPermissionService
    {

        Task<ResponseValidityModel> Save(PermissionModel model);
        Task<List<PermissionViewModel>> GetAll();
        Task<PermissionViewModel> GetById(int Id);
        Task<PermissionViewModel> GetByName(string Name);
        Task<ResponseValidityModel> Delete(int Id);
    }
}
