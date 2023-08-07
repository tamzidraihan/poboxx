using SmartBox.Business.Core.Models.Company;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Role
{
    public interface IRoleService
    {

        Task<ResponseValidityModel> Save(RoleModel model);
        Task <List<RoleViewModel>> GetAll(); 
        Task<RoleViewModel> GetById(int Id);
        Task<RoleViewModel> GetByName(string Name);
        Task<ResponseValidityModel> Delete(int Id);
        
    }
}
