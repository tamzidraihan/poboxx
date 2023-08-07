using SmartBox.Business.Core.Models.CompanyUser;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.CompanyUser
{
    public interface ICompanyUserService
    {
        Task<ResponseValidityModel> ActivateCompanyUser(ActivatonCompanyUserModel activatonCompanyUserModel, bool isSystemGenerated);
        Task<ResponseValidityModel> SaveCompanyUser(PostCompanyUserModel postCompanyUserModel);
        Task<ResponseValidityModel> UpdateCompanyUser(PostUpdateCompanyUserModel postUpdateCompanyUserModel);
        Task<List<CompanyUserViewModel>> GetCompanyUser(string userKeyId = null, string username = null,
                                             string email = null, bool? isDeleted = null,
                                             bool? isActivated = null);
        Task<AuthenticationModel> GetCompanyLogInUser(CompanyUserLogInModel logInModel);
        Task<ResponseValidityModel> DeleteCompanyUser(int id);
        Task<ResponseValidityModel> ActivateDeactivate(int id, int? isActive);
    }
}
