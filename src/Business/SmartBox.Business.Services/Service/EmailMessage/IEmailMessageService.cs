using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.EmailMessage
{
    public interface IEmailMessageService
    {
        Task<ResponseValidityModel> Save(EmailModel model);
    }
}
