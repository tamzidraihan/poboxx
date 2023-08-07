using SendGrid.Helpers.Mail;
using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.Email;
using SmartBox.Business.Core.Models.Logger;
using SmartBox.Business.Core.Models.MessageModel;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Notification
{
    public interface INotificationService
    {
        Task<bool> SendSMS(string phoneNumber, string message, string refCode);
        Task<MessageModel> GetEmailMessage(int emailMessageId);
        Task<List<MessageLogModel>> GetMessageLogs(int? companyId = null, int? currentPage = null, int? pageSize = null);
        Task<bool> SendBookingReceiptEmail(BookingReceiptModel model);
        Task<FCMResponseModel> SendFCMNotificationAsync(FCMNotificationRequest request);
        Task<bool> SendSmtpEmailAsync(EmailModel model, int? companyId = null);
    }
}
