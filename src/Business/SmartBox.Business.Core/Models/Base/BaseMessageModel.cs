using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;

namespace SmartBox.Business.Core.Models.Base
{
    public class BaseMessageModel : IBaseMessageModel
    {
        public string Message { get; set; }
        public int MessageReturnNumber { get; set; }
        public string ReturnStatusType { get; set; }
    }
}
