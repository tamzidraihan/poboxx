using System;
using System.Threading.Tasks;
using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.AppMessage;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.AppMessage;
using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Feedback;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.AppMessage;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.AppMessage
{
    public class AppMessageService : IAppMessageService
    {
        private readonly IAppMessageRepository _appMessageRepository;
        public AppMessageService(IAppMessageRepository appMessageRepository)
        {
            
            _appMessageRepository = appMessageRepository;
        }
          
        public async Task<string> GetApplicationMessage(int number)
        {
            var message = await _appMessageRepository.GetApplicationMessage(number);
            return message.Message;
        }

        static BaseMessageModel MappedMessage(string message, int returnNumber)
        {
            var model = new BaseMessageModel
            {
                Message = message,
                MessageReturnNumber = returnNumber,
                ReturnStatusType = returnNumber >= 0
                    ? GlobalEnums.AppStatusType.Information.GetAppStatusEnumName()
                    : GlobalEnums.AppStatusType.Error.GetAppStatusEnumName()
            };

            return model;
        }

        public BaseMessageModel SetMessage(int returnNumber)
        {
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }

    

        public BaseMessageModel SetMessageWithTextReplace(int returnNumber, string textReplace)
        {
            var message = GetApplicationMessage(returnNumber).Result;

            message = message.Replace(GlobalConstants.MessageParameters.Field, textReplace);

            return MappedMessage(message, returnNumber);
        }

        public BaseMessageModel SetSystemErrorMessage()
        {
            short returnNumber = -600;
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }

        public BaseMessageModel SetFailUpdateMessage()
        {
            short returnNumber = -603;
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }

        public BaseMessageModel SetSuccessAddedMessage()
        {
            short returnNumber = 500;
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }

        public BaseMessageModel SetFoundMessage()
        {
            short returnNumber = 504;
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }

        public BaseMessageModel SetSuccessUpdateMessage()
        {
            short returnNumber = 501;
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }

        public BaseMessageModel SetNotFoundMessage()
        {
            short returnNumber = 503;
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }

        public BaseMessageModel SetRecordActivated()
        {
            short returnNumber = 508;
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }

        public BaseMessageModel SetRecordDeactivated()
        {
            short returnNumber = 507;
            var message = GetApplicationMessage(returnNumber).Result;

            return MappedMessage(message, returnNumber);
        }
 
    }
}
