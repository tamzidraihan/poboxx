using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public class M360ResponseModel
    {
        public int code { get; set; }
        public string name { get; set; }
        public string transid { get; set; }
        public string timestamp { get; set; }
        public int msgcount { get; set; }
        public int telco_id { get; set; }
        public string messageId { get; set; }
    }

}
