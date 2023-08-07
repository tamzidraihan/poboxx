using SmartBox.Business.Core.Entities.ParentCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.ParentCompany
{
    public interface IParentCompanyRepository
    {
        Task<bool> CheckParentCompanyKeyId(string parentCompanyKeyId);
        Task<bool> CheckParentCompanyName(string parentCompanyName);
        Task<bool> CheckParentCompanyId(int parentCompanyId);
        Task<int> GetLastIdentity();
        Task<List<ParentCompanyEntity>> GetParentCompany(bool isAndOperator = true, string parentCompanyKeyId = null, string parentCompanyName = null);
        Task<int> SaveParentCompany(ParentCompanyEntity parentCompanyEntity, bool isInsert);

    }
}
