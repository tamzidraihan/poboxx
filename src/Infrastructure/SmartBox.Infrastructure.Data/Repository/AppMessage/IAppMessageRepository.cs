using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Entities.AppMessage;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Logs;
using SmartBox.Infrastructure.Data.Repository.Base;


namespace SmartBox.Infrastructure.Data.Repository.AppMessage
{
    public interface IAppMessageRepository : IGenericRepositoryBase<ApplicationMessageEntity>
    {
        Task<List<ApplicationMessageEntity>> GetAll();
        Task<int> Create(ApplicationMessageEntity appMessage);
        Task<int> Update(ApplicationMessageEntity appMessage);
        Task<ApplicationMessageEntity> GetApplicationMessage(int applicationMessageId);
        Task<int> DeleteApplicationMessage(int Id);
    }
}
