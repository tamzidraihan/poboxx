using SmartBox.Business.Core.Entities.CompanyUser;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.CompanyUser
{
    public interface ICompanyUserRepository: IGenericRepositoryBase<CompanyUserEntity>
    {
        Task<List<CompanyUserEntity>> GetCompanyUser(string userKeyId = null, string username = null,
                                              string email = null, bool? isDeleted = null, 
                                              bool? isActivated = null, bool? isSystemGenerated = null);
        Task<int> Delete(int id);
        Task<int> SaveCompanyUser(CompanyUserEntity companyUserEntity, bool isInsert);
        Task<int> UpdateCompanyUserPassword(string username, string password);
        Task<int> ActivateCompanyUser(CompanyUserEntity companyUserEntity, bool? isSystemGenerated);
        int GetLastIdentity();
        bool? IsSystemGenerated(bool isSystemGenerated, string userKeyId = null, string username = null);
        Task<int> ActivateDeactivate(int Id, int? isActive);
    }
}
