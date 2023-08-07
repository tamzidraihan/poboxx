using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Services.Service.LogIn;
using SmartBox.Business.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Notification.PushNotification
{
    public class FCMSender : IFCMSender
    {
        private readonly FirebaseConfiguration firebaseConfiguration;
        private readonly ILogger<LogInService> _logger;

        public FCMSender(IOptions<FirebaseConfiguration> options, ILogger<LogInService> logger)
        {
            firebaseConfiguration = options.Value;
            _logger = logger;
        }


        public Task<FCMResponseModel> SendAsync(string deviceToken, object payload, CancellationToken cancellationToken = default)
        {

            var jsonObject = JObject.FromObject(payload);
            jsonObject.Remove("to");
            jsonObject.Add("to", JToken.FromObject(deviceToken));

            return SendAsync(jsonObject, cancellationToken);
        }

        /// <summary>
        /// Send firebase notification.
        /// Please check out payload formats:
        /// https://firebase.google.com/docs/cloud-messaging/concept-options#notifications
        /// The SendAsync method will add/replace "to" value with deviceId
        /// </summary>
        /// <param name="payload">Notification payload that will be serialized using Newtonsoft.Json package</param>
        /// <exception cref="HttpRequestException">Throws exception when not successful</exception>
        private async Task<FCMResponseModel> SendAsync(object payload, CancellationToken cancellationToken = default)
        {
            try
            {
                HttpClient http = new HttpClient();
                var serialized = JsonConvert.SerializeObject(payload);

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, firebaseConfiguration.FcmUrl))
                {var FBServerKey = System.Environment.GetEnvironmentVariable("FB_SERVER_KEY");
                    httpRequest.Headers.Add("Authorization", $"key = {FBServerKey}");

                    if (!string.IsNullOrEmpty(firebaseConfiguration.SenderId))
                    {
                        httpRequest.Headers.Add("Sender", $"id = {firebaseConfiguration.SenderId}");
                    }

                    httpRequest.Content = new StringContent(serialized, Encoding.UTF8, "application/json");

                    using (var response = await http.SendAsync(httpRequest, cancellationToken))
                    {
                        var responseString = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError("Error sending FCM Push Notification: " + responseString);
                            throw new HttpRequestException("Firebase notification error: " + responseString);
                        }

                        var fcmResponse = JsonConvert.DeserializeObject<FcmResponse>(responseString);

                        _logger.LogInformation("Successfully send FCM Push Notification with payload: " + payload.ToString());
                        return new FCMResponseModel { IsSuccess = fcmResponse.IsSuccess() };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending FCM Push Notification: " + ex.Message);

                return new FCMResponseModel { IsSuccess = false };
            }
        }
    }
}
