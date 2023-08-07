using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using SmartBox.Business.Services.Service.LogIn;
using SmartBox.Business.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Notification.PushNotification
{
    public class ApnSender : IApnSender
    {
        private static readonly ConcurrentDictionary<string, Tuple<string, DateTime>> tokens = new ConcurrentDictionary<string, Tuple<string, DateTime>>();

        private readonly FirebaseConfiguration firebaseConfiguration;
        private readonly ILogger<LogInService> _logger;
        /// <summary>
        /// Apple push notification sender constructor
        /// </summary>
        public ApnSender(IOptions<FirebaseConfiguration> options, ILogger<LogInService> logger)
        {
            firebaseConfiguration = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Serialize and send notification to APN. Please see how your message should be formatted here:
        /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW1
        /// Payload will be serialized using Newtonsoft.Json package.
        /// !IMPORTANT: If you send many messages at once, make sure to retry those calls. Apple typically doesn't like 
        /// to receive too many requests and may occasionally respond with HTTP 429. Just try/catch this call and retry as needed.
        /// </summary>
        /// <exception cref="HttpRequestException">Throws exception when not successful</exception>
        public async Task<ApnsResponse> SendAsync(
            object notification,
            string deviceToken,
            string apnsId = null,
            int apnsExpiration = 0,
            int apnsPriority = 10,
            bool isBackground = false,
            CancellationToken cancellationToken = default)
        {
            var path = $"/3/device/{deviceToken}";
            var json = JsonConvert.SerializeObject(notification);
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(firebaseConfiguration.BaseURL);
            using (var message = new HttpRequestMessage(HttpMethod.Post, path))
            {
                message.Version = new Version(2, 0);
                message.Content = new StringContent(json);

                message.Headers.Authorization = new AuthenticationHeaderValue("bearer", GetJwtToken());
                message.Headers.TryAddWithoutValidation(":method", "POST");
                message.Headers.TryAddWithoutValidation(":path", path);
                message.Headers.Add("apns-topic", firebaseConfiguration.AppBundleIdentifier);
                message.Headers.Add("apns-expiration", apnsExpiration.ToString());
                message.Headers.Add("apns-priority", apnsPriority.ToString());
                message.Headers.Add("apns-push-type", isBackground ? "background" : "alert"); // required for iOS 13+

                if (!string.IsNullOrWhiteSpace(apnsId))
                {
                    message.Headers.Add(firebaseConfiguration.ApnIdHeader, apnsId);
                }

                using (var response = await httpClient.SendAsync(message, cancellationToken))
                {
                    var succeed = response.IsSuccessStatusCode;
                    var content = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ApnsError>(content);


                    if (error != null)
                    {
                        _logger.LogError("Error sending iOS Push Notification: " + error.Reason + " for device " + deviceToken);


                    }
                    else
                        _logger.LogInformation("Successfully sent iOS Push Notification for device: " + deviceToken);

                    return new ApnsResponse
                    {
                        IsSuccess = succeed,
                        Error = error
                    };
                }
            }
        }

        private string GetJwtToken()
        {
            var (token, date) = tokens.GetOrAdd(firebaseConfiguration.AppBundleIdentifier, _ => new Tuple<string, DateTime>(CreateJwtToken(), DateTime.UtcNow));
            if (date < DateTime.UtcNow.AddMinutes(-firebaseConfiguration.TokenExpiresMinutes))
            {
                tokens.TryRemove(firebaseConfiguration.AppBundleIdentifier, out _);
                return GetJwtToken();
            }

            return token;
        }

        private string CreateJwtToken()
        {
            var P8PrivateKeyId = firebaseConfiguration.P8PrivateKeyId;// System.Environment.GetEnvironmentVariable("P8_PRIVATE_KEY_ID");
            var header = JsonConvert.SerializeObject(new { alg = "ES256", kid = CleanP8Key(P8PrivateKeyId) });
            var payload = JsonConvert.SerializeObject(new { iss = firebaseConfiguration.TeamId, iat = EpochTime() });
            var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
            var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
            var unsignedJwtData = $"{headerBase64}.{payloadBase64}";
            var unsignedJwtBytes = Encoding.UTF8.GetBytes(unsignedJwtData);

            var P8PrivateKey = firebaseConfiguration.P8PrivateKey;// System.Environment.GetEnvironmentVariable("P8_PRIVATE_KEY");
            using (var dsa = AppleCryptoHelper.GetEllipticCurveAlgorithm(CleanP8Key(P8PrivateKey)))
            {
                var signature = dsa.SignData(unsignedJwtBytes, 0, unsignedJwtBytes.Length, HashAlgorithmName.SHA256);
                return $"{unsignedJwtData}.{Convert.ToBase64String(signature)}";
            }
        }

        private static int EpochTime()
        {
            var span = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Convert.ToInt32(span.TotalSeconds);
        }

        private static string CleanP8Key(string p8Key)
        {
            // If we have an empty p8Key, then don't bother doing any tasks.
            if (string.IsNullOrEmpty(p8Key))
            {
                return p8Key;
            }

            var lines = p8Key.Split('\n').ToList();

            if (0 != lines.Count && lines[0].StartsWith("-----BEGIN PRIVATE KEY-----"))
            {
                lines.RemoveAt(0);
            }

            if (0 != lines.Count && lines[lines.Count - 1].StartsWith("-----END PRIVATE KEY-----"))
            {
                lines.RemoveAt(lines.Count - 1);
            }

            var result = string.Join(string.Empty, lines);

            return result;
        }
    }
}
