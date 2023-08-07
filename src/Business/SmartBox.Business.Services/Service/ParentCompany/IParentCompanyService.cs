using SmartBox.Business.Core.Models.ParentCompany;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.ParentCompany
{
    public interface IParentCompanyService
    {
        Task<bool> CheckParentCompanyKeyId(string parentCompanyKeyId);
        Task<bool> CheckParentCompanyName(string parentCompanyName);
        Task<ResponseValidityModel> SetParentCompany(ParentCompanyPostModel parentCompanyPostModel);
        Task<List<ParentCompanyViewModel>> GetParentCompany(bool isAndOperator = true, string parentCompanyKeyId = null, string parentCompanyName=null);

    }
}
