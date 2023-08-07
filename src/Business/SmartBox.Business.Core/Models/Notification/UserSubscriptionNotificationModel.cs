using SmartBox.Business.Core.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public class UserSubscriptionNotificationModel
    {
       public string UserEmail { get; set; }
        public string Token { get; set; }
        public DeviceType DeviceType { get; set; }
        public string MessageTitle { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public string JsonData { get; set; }
    }
}
