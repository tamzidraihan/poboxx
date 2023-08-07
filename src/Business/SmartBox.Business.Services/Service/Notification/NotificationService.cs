using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Business.Core.Entities.Logs;
using SmartBox.Business.Core.Models.Email;
using SmartBox.Business.Core.Models.Logger;
using SmartBox.Business.Core.Models.MessageModel;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Notification.PushNotification;
using SmartBox.Infrastructure.Data.Repository.EmailMessage;
using SmartBox.Infrastructure.Data.Repository.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using SmartBox.Business.Shared;
using Microsoft.Extensions.Hosting;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Shared.Models;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using System.Net.Mail;
using System.ComponentModel.Design;
using System.Linq.Expressions;

namespace SmartBox.Business.Services.Service.Notification
{
    public class NotificationService : BaseMessageService<NotificationService>, INotificationService
    {
        private readonly SMSOptionModel _smsOptionModel;
        private readonly HangfireConfig hangfireConfig;
        private readonly EmailSettingsModel _emailSettingsModel;
        private readonly ILogger _logger;
        private readonly IEmailMessageRepository _emailMessageRepository;
        static string From;
        static string SenderName;
        private readonly IFCMProvider fCMProvider;
        public readonly IMessageLogRepository messageLogRepository;
        private readonly IHostEnvironment hostEnvironment;
        private readonly GlobalConfigurations globalConfigurations;
        private readonly SMTPConfiguration smtpConfiguration;

        public NotificationService(IAppMessageService appMessageService, IMapper mapper,
                                    IOptions<SMSOptionModel> smsOptionModel, ILogger<NotificationService> logger,
                                    IOptions<EmailSettingsModel> emailSettingsModel,
                                    IFCMProvider fCMProvider,
                                    IOptions<GlobalConfigurations> globalConfigurations,
                                    IOptions<HangfireConfig> hangfireConfig,
                                    IOptions<SMTPConfiguration> smtpConfiguration,
                                    IMessageLogRepository messageLogRepository,
                                    IHostEnvironment hostEnvironment,
                                    IEmailMessageRepository emailMessageRepository) : base(appMessageService, mapper)
        {
            _smsOptionModel = smsOptionModel.Value;
            _logger = logger;
            this.hangfireConfig = hangfireConfig.Value;
            _emailSettingsModel = emailSettingsModel.Value;
            _emailMessageRepository = emailMessageRepository;
            this.fCMProvider = fCMProvider;
            this.messageLogRepository = messageLogRepository;
            From = _emailSettingsModel.From;
            SenderName = _emailSettingsModel.SenderName;
            this.hostEnvironment = hostEnvironment;
            this.globalConfigurations = globalConfigurations.Value;
            this.smtpConfiguration = smtpConfiguration.Value;
        }
        public async Task<FCMResponseModel> SendFCMNotificationAsync(FCMNotificationRequest request)
        {
            if (!hangfireConfig.PushNotificationEnable) return new FCMResponseModel { IsSuccess = false };
            var response = await fCMProvider.SendAsync(request.token, request.deviceType, request.title, request.notificationBody, request.clickAction, request.imgUrl, request.json, request.type, request.commonId);
            await messageLogRepository.Save(
                new MessageLogEntity
                {
                    DateCreated = DateTime.Now,
                    isSent = response.IsSuccess,
                    Type = MessageLogType.PushNotification,
                    Message = JsonConvert.SerializeObject(request),
                    Receipent = request.token,
                    Subject = request.title
                });
            return response;
        }
        private async Task<bool> SendEmail(EmailModel model, int? companyId = null)
        {
            if (!hangfireConfig.EmailNotificationEnable) return false;
            try
            {
                if (string.IsNullOrEmpty(model.To) ||
                string.IsNullOrEmpty(model.Message) ||
                string.IsNullOrEmpty(model.Subject) ||
                 !SharedServices.IsValidEmail(model.To)) return false;
                var from = new EmailAddress { Email = From };
                var lst = model.To.Split(';');

       

                var receipents = new List<EmailAddress>();
                foreach (var receipent in lst)

                {
                    if (!string.IsNullOrEmpty(receipent))
                        receipents.Add(new EmailAddress { Email = receipent });
                }
                if (receipents.Count > 0)
                {
                    return await SendSmtpEmailAsync(model);
                }
                else return false;
            }
            catch { return false; }
        }
        private static SendGridMessage BuildSendgridEmailLetter(SendEmailModel sendEmailModel, string plainBody, string htmlBody = null, SendGrid.Helpers.Mail.Attachment attachment = null)
        {
            var from = new EmailAddress(From, SenderName);
            var to = new EmailAddress(sendEmailModel.To, sendEmailModel.ReceiverName);
            var plainTextContent = plainBody;
            var htmlContent = htmlBody;
            var msg = MailHelper.CreateSingleEmail(from, to, sendEmailModel.Subject, plainTextContent, htmlContent);
            return msg;
        }

        public async Task<MessageModel> GetEmailMessage(int emailMessageId)
        {
            var dbModel = await _emailMessageRepository.GetEmailMessage(emailMessageId);
            var model = Mapper.Map<MessageModel>(dbModel);
            return model;
        }

        private async Task<bool> SendgridSendEmail(SendGridMessage sendGridMessage, int? companyId = null)
        {
            var client = new SendGridClient(_emailSettingsModel.Key);
            var response = await client.SendEmailAsync(sendGridMessage);
            string log = response.IsSuccessStatusCode ? "Sendgrid successful email sending" : "Sendgrid failed email sending";
            _logger.LogInformation($"{log} {nameof(SendGridMessage.Subject)}:{sendGridMessage.Subject}, " +
                                         $"EmailAddress:{sendGridMessage.Personalizations[0].Tos[0].Email}, " +
                                         $"RecepientName:{sendGridMessage.Personalizations[0].Tos[0].Name}, ");
            var receipents = sendGridMessage.Personalizations?.Select(s => s.Tos).FirstOrDefault();

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendSMS(string phoneNumber, string message, string refCode)
        {
            try
            {
                var url = _smsOptionModel.Host;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                //var data = new
                //{
                //    username = "\"" + _smsOptionModel.Username + "\"",
                //    password = "\"" + _smsOptionModel.Password + "\"",
                //    msisdn = "\"" + phoneNumber + "\"",
                //    shortcode_mask = "\"" + _smsOptionModel.ShortcodeMask + "\"",
                //    content = "\"" + message + "\"",
                //    rcvd_transid = "\"" + refCode + "\""
                //};

                //string stringPlants = "\"Swamp Cabbage\"";

                var data = string.Concat("{", "\"username\":", "\"" + _smsOptionModel.Username + "\"", ",",
                                             "\"password\":", "\"" + _smsOptionModel.Password + "\"", ",",
                                             "\"msisdn\":", "\"" + phoneNumber + "\"", ",",
                                             "\"shortcode_mask\":", "\"" + _smsOptionModel.ShortcodeMask + "\"", ",",
                                             "\"content\":", "\"" + message + "\"", ",",
                                             "\"rcvd_transid\":", "\"" + refCode + "\"",
                                          "}");



                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = await streamReader.ReadToEndAsync();
                    var json = JsonConvert.DeserializeObject<SMSJsonResultModel>(result);
                    _logger.LogInformation($"Successfully sent SMS {nameof(SMSJsonResultModel.Code)}:{json.Code}, " +
                                           $"{nameof(SMSJsonResultModel.Name)}:{json.Name}, " +
                                           $"{nameof(SMSJsonResultModel.TransId)}:{json.TransId}, " +
                                           $"{nameof(SMSJsonResultModel.TimeStamp)}:{json.TimeStamp}, " +
                                           $"{nameof(SMSJsonResultModel.MsgCount)}:{json.MsgCount}, " +
                                           $"{nameof(SMSJsonResultModel.Telco_Id)}:{json.Telco_Id}, " +
                                           $" Phone number: {phoneNumber}");
                }

                return true;
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
        public async Task<List<MessageLogModel>> GetMessageLogs(int? companyId = null, int? currentPage = null, int? pageSize = null)
        {
            var dbModel = await messageLogRepository.Get(companyId, currentPage, pageSize);
            var model = new List<MessageLogModel>();

            if (dbModel != null)
                model = Mapper.Map<List<MessageLogModel>>(dbModel);
            return model;
        }
        public async Task<bool> SendBookingReceiptEmail(BookingReceiptModel model)
        {
            var fileName = "wwwroot/emails/BookingReceipt.html";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await SharedServices.ReadFile(fullPath);
            if (response.isError) return false;

            var companyLogoUrl = $"{globalConfigurations.BaseApiUrl}/img/logo.png";
            var locationIconUrl = $"{globalConfigurations.BaseApiUrl}/img/location-pin.png";
            var calendarIconUrl = $"{globalConfigurations.BaseApiUrl}/img/calendar.png";
            var lockerIconUrl = $"{globalConfigurations.BaseApiUrl}/img/locker.png";
            var referenceNoIconUrl = $"{globalConfigurations.BaseApiUrl}/img/reference-no.png";
            var userIconUrl = $"{globalConfigurations.BaseApiUrl}/img/user.png";
            var paymentIconUrl = $"{globalConfigurations.BaseApiUrl}/img/payment.png";

            var html = $"";
            foreach (var item in model.Detail)
            {
                html += "<tr><td colspan='3'><hr/></td></tr>";
                html += $"<tr><th class='left-header'><ul><li class='bold'>";
                html += $"<img class='w-3' src='{calendarIconUrl}' />  Storage Date Duration</li>";
                html += $"<li class='m-18'><span class='bold'>From:</span> {item.FromDate}</li>";
                html += $"<li class='m-18'><span class='bold'>To: </span> {item.ToDate}</li></ul>";
                html += "</th><th></th><th class='right-header'><ul><li>";
                html += $"<img class='w-4' src='{locationIconUrl}'/><span class='bold'>Location: </span>{item.Location}</li>";
                html += $"<li class='m-18'><img class='w-4' src='{lockerIconUrl}'/><span class='bold'>Locker Size:</span>{item.LockerSize}</li>";
                html += "</ul></th></tr> <tr style='background-color:white'><td class='p-10' colspan='2'><ul>";
                html += $"<li class='bold'><img class='w-3' src='{userIconUrl}' />   Receiver Detail</li>";
                html += $"<li class='m-18'><span class='bold'>Name:</span>{item.ReceiverName}</li>";
                html += $"<li class='m-18'><span class='bold'>Mobile Number:</span> {item.ReceiverMobileNo}</li>";
                html += $"<li><img class='w-3' src='{referenceNoIconUrl}'/><span class='bold'>Reference Number:</span>{item.referenceNo}</li></ul></td>";
                html += $"<td><ul><li class='bold'><img class='w-4' src='{paymentIconUrl}'/>Payment Detail</li>";
                foreach (var breakdown in item.PriceBreakdown)
                {
                    html += $"<li class='m-18'><span class='bold'>{breakdown.Key}({model.Currency}):</span>{breakdown.Value.ToString("0.00")}<hr class='m-r-25'/></li>";
                }
                html += $"<li class='m-18'><span class='bold'>Total Price({model.Currency}):</span>{item.TotalPrice.ToString("0.00")}</li>";
                html += "</ul></td></tr>";
            }
            response.Content = response.Content.Replace("{LOGO-IMAGE-URL}", companyLogoUrl)
                .Replace("{COMPANY-NAME}", globalConfigurations.AppName)
                .Replace("{NAME}", model.Name)
                .Replace("{CONTACT-NO}", model.ContactNo)
                .Replace("{CURRENCY}", model.Currency)
                .Replace("{TOTAL-PRICE}", model.TotalPrice.ToString("0.00"))
                .Replace("{BODY}", html);
            return await SendEmail(new EmailModel
            {
                Type = MessageType.HtmlContent,
                Message = response.Content,
                To = model.UserEmail,
                Subject = globalConfigurations.ReceiptEmailSubject
            });
        }

        public async Task<bool> SendSmtpEmailAsync(EmailModel model, int? companyId = null)
        {
            var receipents = new List<string>();
            var ccReceipients = new List<string>();
            bool isSent = false;
            var smtpPassword = System.Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            try
            {
                if (!hangfireConfig.EmailNotificationEnable) return false;
                if (string.IsNullOrEmpty(model.To) ||
                string.IsNullOrEmpty(model.Message) ||
                string.IsNullOrEmpty(model.Subject) ||
                 !SharedServices.IsValidEmail(model.To)) return false;

                var lst = model.To.Split(';');
             

                foreach (var receipent in lst)
                {
                    if (!string.IsNullOrEmpty(receipent))
                        receipents.Add(receipent);
                }

                if (!string.IsNullOrEmpty(model.CC))
                {
                    var lstCC = model.CC.Split(';');

                    foreach (var ccReceipent in lstCC)
                    {
                        if (!string.IsNullOrEmpty(ccReceipent))
                            ccReceipients.Add(ccReceipent);
                    }
                }    

                if (receipents.Count > 0)
                {
                    using (var message = new MailMessage())
                    {
                        foreach (var toAddress in receipents)
                        {
                            message.To.Add(toAddress);
                        }
                        foreach (var toCc in ccReceipients)
                        {
                            message.CC.Add(toCc);
                        }
                        
                        message.Subject = model.Subject;
                        message.Body = model.Message;
                        message.IsBodyHtml = (model.Type == MessageType.HtmlContent ? true : false);
                        message.From = new MailAddress(smtpConfiguration.FromAddress);
               

                        using (var client = new SmtpClient(smtpConfiguration.Server, smtpConfiguration.Port))
                        {
                            client.EnableSsl = true;
                            client.UseDefaultCredentials = false;
                            client.Credentials = new NetworkCredential(smtpConfiguration.UserName, smtpPassword);
                            client.Send(message);
                            isSent = true;
                        }
                    }
                }
            }
            catch { isSent = false; }
            if (companyId.HasValue && receipents.Count > 0)
            {
                await messageLogRepository.Save(new MessageLogEntity
                {
                    CompanyId = companyId,
                    Sender = smtpConfiguration.FromAddress,
                    isSent = isSent,
                    Message = model.Message,
                    Subject = model.Subject,
                    Receipent = string.Join(",", receipents.Select(s => s)),
                    Type = MessageLogType.Email
                });
            }
            return isSent;
        }
    }
}
