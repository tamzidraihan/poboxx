using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Shared;

namespace SmartBox.Business.Core
{
    public static class ModelExtension
    {
        #region MappedBasedMessageModel
        public static ResponseValidityModel MappedResponseValidityModel(this BaseMessageModel source,  List<string> messageList = null)
        {
            var response = new ResponseValidityModel
            {
                MessagesList = messageList,
                Message = source.Message,
                MessageReturnNumber = source.MessageReturnNumber,
                ReturnStatusType = source.ReturnStatusType
            };
    
            return response;
        }

        #endregion
    }
}
