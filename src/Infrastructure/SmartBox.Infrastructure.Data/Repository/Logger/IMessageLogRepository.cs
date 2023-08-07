using SmartBox.Business.Core.Entities.Logs;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Logs
{
    public interface IMessageLogRepository : IGenericRepositoryBase<MessageLogEntity>
    {
        Task<List<MessageLogEntity>> Get(int? companyId = null,int ? currentPage = null, int? pageSize = null);
        Task<int> Save(MessageLogEntity model);
    }
}
