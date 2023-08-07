using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Notification.PushNotification
{
    public interface IFCMProvider
    {
        Task<FCMResponseModel> SendAsync(string token, DeviceType deviceType, string title, string notificationBody, string clickAction, string imgUrl, string json, string type, string uniqueId);
    }
}
