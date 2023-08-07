using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Models.AppMessage;
using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.ResponseValidity;

namespace SmartBox.Business.Services.Service.AppMessage
{
    public interface IAppMessageService
    {

     
        Task<string> GetApplicationMessage(int number);
        BaseMessageModel SetMessage(int returnNumber);
        BaseMessageModel SetMessageWithTextReplace(int returnNumber, string textReplace);
        BaseMessageModel SetSystemErrorMessage();
        BaseMessageModel SetFoundMessage();
        //BaseMessageModel SetNotificationSuccessMessage();
        BaseMessageModel SetSuccessUpdateMessage();
        BaseMessageModel SetFailUpdateMessage();
        BaseMessageModel SetNotFoundMessage(); 
        
    }
}
