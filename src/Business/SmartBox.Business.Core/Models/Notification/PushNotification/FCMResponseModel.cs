using SmartBox.Business.Core.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification.PushNotification
{
    public class FCMResponseModel
    {
        public bool IsSuccess { get; set; }
    }
    public class FCMNotificationRequest
    {
        public string token { get; set; }
        public DeviceType deviceType { get; set; }
        public string title { get; set; }
        public string notificationBody { get; set; }
        public string clickAction { get; set; } = "OPEN_APP";
        public string imgUrl { get; set; }
        public string json { get; set; }
        public string type { get; set; }
        public string commonId { get; set; }
    }
}
