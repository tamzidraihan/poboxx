using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Email;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Feedback;
using SmartBox.Infrastructure.Data.Repository.EmailMessage;
using SmartBox.Infrastructure.Data.Repository.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.EmailMessage
{
    public class EmailMessageService : BaseMessageService<EmailMessageService>, IEmailMessageService
    {
        private readonly IEmailMessageRepository _emailMessageRepository;
        public EmailMessageService(IAppMessageService appMessageService, IMapper mapper, IEmailMessageRepository emailMessageRepository) : base(appMessageService, mapper)
        {
            _emailMessageRepository = emailMessageRepository;
        }

        public async Task<ResponseValidityModel> Save(EmailModel model)
        {
            var dataModel =  ValidateEmailMessage(model);

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<EmailModel, EmailEntity>(model);
                var ret = await _emailMessageRepository.Save(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }

        ResponseValidityModel ValidateEmailMessage(EmailModel emailModel)
        {
            var model = new ResponseValidityModel();


            return model;
        }

    }
}
