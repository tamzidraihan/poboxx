using SmartBox.Business.Core.Models.Company;
using SmartBox.Business.Core.Models.CompanyUser;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Company
{
    public interface ICompanyService
    {
        Task<List<CompanyViewModel>> GetCompanyList(string companyKeyId = null);
        Task<ResponseValidityCompanyModel> SaveCompany(CompanyModel companyModel);
        Task<ResponseValidityModel> SetCompanyActivation(int id, bool isDeleted);
        Task<List<CompanyCabinetViewModel>> GetCompanyCabinets(int? companyId = null, int? cabinetId = null, bool? unAssignedOnly = null);
        Task<ResponseValidityModel> SaveCompanyCabinet(CompanyCabinetModel param);
        Task<ResponseValidityModel> AssignCompanyCabinetToLocation(AssignCompanyCabinetModel param);
        Task<ResponseValidityModel> UnAssignCompanyCabinetToLocation(AssignCompanyCabinetModel param);
        Task<List<UnassignCompanyModel>> GetUnAssignedCompanies(int? companyId = null);
        Task<List<UnassignedCompanyCabinetModel>> GetUnAssignedCompanyCabinets(int? cabinetId = null);
    }
}
