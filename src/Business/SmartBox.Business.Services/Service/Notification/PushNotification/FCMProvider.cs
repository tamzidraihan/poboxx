using Microsoft.Extensions.Options;
using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using SmartBox.Business.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Notification.PushNotification
{
    public class FCMProvider : IFCMProvider
    {
        private readonly IFCMSender fCMSender;
        private readonly IApnSender apnSender;
        private readonly HangfireConfig hangfireConfig;
        public FCMProvider(IOptions<HangfireConfig> hangfireConfig,
            IFCMSender fCMSender, IApnSender apnSender)
        {
            this.fCMSender = fCMSender;
            this.apnSender = apnSender;
            this.hangfireConfig = hangfireConfig.Value;
        }
        /// <summary>
        /// send FCM Notification
        /// </summary>
        /// <param name="token"></param>
        /// <param name="deviceType"></param>
        /// <param name="title"></param>
        /// <param name="notificationBody"></param>
        /// <param name="clickAction"></param>
        /// <param name="imgUrl"></param>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <param name="commonId"></param>
        /// <returns>FCMResponseModel</returns>
        public async Task<FCMResponseModel> SendAsync(string token, DeviceType deviceType, string title, string notificationBody, string clickAction, string imgUrl, string json, string type, string commonId)
        {
            if (!hangfireConfig.PushNotificationEnable) return null;
            if (string.IsNullOrEmpty(token)) return new FCMResponseModel { IsSuccess = false };
            if (deviceType == DeviceType.Web || deviceType == DeviceType.Android)
            {
                var payload = new
                {
                    notification = new
                    {
                        body = notificationBody,
                        title = title,
                        click_action = clickAction,
                        //icon = $"/img/logo/icon.png",
                        //image = imgUrl,
                        tag = commonId
                    },
                    data = new
                    {
                        body = json,
                        targeturl = clickAction,
                        type = type
                    }
                };
                var response = await fCMSender.SendAsync(token, payload);
                return new FCMResponseModel { IsSuccess = response.IsSuccess };
            }
            else if (deviceType == DeviceType.IOS)
            {
                var payload = new AppleNotification(
                    Guid.NewGuid(),
                    notificationBody,
                    json,
                   title);
                var response = await apnSender.SendAsync(payload, token);
                return new FCMResponseModel { IsSuccess = response.IsSuccess };
            }
            return new FCMResponseModel { IsSuccess = false };
        }
    }
}
