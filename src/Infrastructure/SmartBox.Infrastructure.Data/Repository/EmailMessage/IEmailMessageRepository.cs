using SmartBox.Business.Core.Entities.AppMessage;
using SmartBox.Business.Core.Entities.Email;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.EmailMessage
{
    public interface IEmailMessageRepository: IGenericRepositoryBase<MessageEntity>
    {
        Task<MessageEntity> GetEmailMessage(int emailMessageId);
        Task<int> Save(EmailEntity email);
    }
}
