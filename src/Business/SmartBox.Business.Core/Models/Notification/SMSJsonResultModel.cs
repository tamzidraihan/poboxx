using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public class SMSJsonResultModel
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string TransId { get; set; }
        public string TimeStamp { get; set; }
        public int MsgCount { get; set; }
        public int Telco_Id { get; set; }


    }
}
