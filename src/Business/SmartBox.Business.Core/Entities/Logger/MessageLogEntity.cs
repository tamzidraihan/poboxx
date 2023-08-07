using SmartBox.Business.Core.Entities.Common;
using SmartBox.Business.Core.Models.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Logs
{
    public enum MessageLogType { Email = 0, PushNotification = 1 }
    public class MessageLogEntity
    {
        public bool isSent { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Receipent { get; set; }
        public string Sender { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CompanyId { get; set; }
        public MessageLogType Type { get; set; }
    }
}
