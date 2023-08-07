using SmartBox.Business.Core.Models.Company;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.Roles;
using SmartBox.Business.Core.Models.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.UserRole
{
    public interface IUserRoleService
    { 
        Task<ResponseValidityModel> Save(UserRoleModel model);
        Task <List<UserRoleViewModel>> GetAll(int userId); 
        Task<UserRoleViewModel> GetById(int Id);
        Task<ResponseValidityModel> Delete(int Id);
        
    }
}
