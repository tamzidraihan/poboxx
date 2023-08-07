using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartBox.Business.Core.Entities.Logs;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Infrastructure.Data.Repository.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.HTTPService
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly SMSOptionModel _smsOptionModel;
        private readonly ILogger _logger;
        public readonly IMessageLogRepository messageLogRepository;
        public HttpService(HttpClient httpClient, IOptions<SMSOptionModel> smsOptionModel,
                            ILogger<HttpService> logger, IMessageLogRepository messageLogRepository)
        {
            _httpClient = httpClient;
            _smsOptionModel = smsOptionModel.Value;
            _logger = logger;
            this.messageLogRepository = messageLogRepository;
        }

        public async Task<bool> SendSMS(string phoneNumber, string message, string refCode)
        {
            try
            {
                SMSPostModel sMSPostModel = new SMSPostModel();
                sMSPostModel.app_key = _smsOptionModel.AppKey;
                sMSPostModel.app_secret = _smsOptionModel.Secret;
                sMSPostModel.msisdn = phoneNumber;
                sMSPostModel.shortcode_mask = _smsOptionModel.ShortcodeMask;
                sMSPostModel.content = message;
                sMSPostModel.rcvd_transid = refCode;

                string json = JsonConvert.SerializeObject(sMSPostModel);
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = _httpClient.PostAsync(_smsOptionModel.Host, httpContent);
                var result = await response.Result.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<M360ResponseModel>(result);
                if (responseModel.code != 201)
                {
                    _logger.LogError($"Unable to send to phone number:{phoneNumber}");
                    await messageLogRepository.Save(
                new MessageLogEntity
                {
                    DateCreated = DateTime.Now,
                    isSent = false,
                    Type = MessageLogType.PushNotification,
                    Message = JsonConvert.SerializeObject(new { message = message, refCode = refCode }),
                    Receipent = phoneNumber,
                    Subject = refCode
                });
                    return false;
                }
                else
                {
                    _logger.LogInformation(responseModel.code + " " + responseModel.name);
                    await messageLogRepository.Save(
                new MessageLogEntity
                {
                    DateCreated = DateTime.Now,
                    isSent = true,
                    Type = MessageLogType.PushNotification,
                    Message = JsonConvert.SerializeObject(new { message = message, refCode = refCode }),
                    Receipent = phoneNumber,
                    Subject = refCode
                });
                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to send to phone number:{phoneNumber}");
                _logger.LogError(ex.Message);

                if (ex.InnerException != null)
                    _logger.LogError(ex.InnerException.Message);

                return false;
            }
        }
    }
}
