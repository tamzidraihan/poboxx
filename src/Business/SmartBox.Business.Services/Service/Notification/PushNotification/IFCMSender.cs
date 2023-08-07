using SmartBox.Business.Core.Models.Notification.PushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Notification.PushNotification
{
    public interface IFCMSender
    {
        Task<FCMResponseModel> SendAsync(string deviceToken, object payload, CancellationToken cancellationToken = default);
    }
}
