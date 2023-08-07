using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Shared;

namespace SmartBox.Business.Core.Models.Base
{
    public interface IBaseMessageModel
    {
        string Message { get; set; }
        int MessageReturnNumber { get; set; }
        string ReturnStatusType { get; set; }

    }
}
