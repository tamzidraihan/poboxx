using SmartBox.Business.Core.Entities.Company;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Company
{
    public interface ICompanyRepository: IGenericRepositoryBase<CompanyEntity>
    {
        Task <List<CompanyLocationEntity>> GetCompany(string companyKeyId = null, int? companyId = null, bool isDeleted = false);
        Task<int> SaveCompany(CompanyEntity companyEntity, bool isInsert);
        Task<int> GetLastIdentity();
        bool CheckCompanyIfExists(int companyId);
    }
}
