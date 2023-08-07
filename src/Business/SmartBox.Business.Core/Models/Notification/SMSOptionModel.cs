using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public class SMSOptionModel
    {
        public string Host { get; set; }
        public string AppKey { get; set; }
        public string Secret { get; set; }
        public string Username { get; set; }
        public string Password  { get; set; }
        public string ShortcodeMask { get; set; }
        
    }
}
