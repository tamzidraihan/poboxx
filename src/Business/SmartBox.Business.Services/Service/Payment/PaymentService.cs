using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Entities.Payment;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.Payment;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.HTTPService;
using SmartBox.Business.Services.Service.Locker;
using SmartBox.Business.Services.Service.Notification;
using SmartBox.Business.Services.Service.User;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.Locker;
using SmartBox.Infrastructure.Data.Repository.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Services.Service.Payment
{
    public class PaymentService : BaseMessageService<PaymentService>, IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILockerRepository lockerRepository;
        private readonly IHttpService _httpService;
        private readonly ILogger<LockerService> _logger;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public PaymentService(IAppMessageService appMessageService,
            ILockerRepository lockerRepository,
            IMapper mapper, IPaymentRepository paymentRepository,
            IHttpService _httpService,
            INotificationService notificationService
            , ILogger<LockerService> logger
            , IUserService userService) : base(appMessageService, mapper)
        {
            _paymentRepository = paymentRepository;
            this.lockerRepository = lockerRepository;
            this._httpService = _httpService;
            _logger = logger;
            this._notificationService = notificationService;
            _userService = userService;

        }

        public async Task<List<PaymentMethodModel>> GetPaymentMethod()
        {
            var dbModel = await _paymentRepository.GetPaymentMethod();
            if (dbModel == null)
                return new List<PaymentMethodModel>();

            var model = Mapper.Map<List<PaymentMethodModel>>(dbModel);

            return model;
        }
        public async Task<PaymentInfoModel> GetPaymentInfoModel(string referenceId)
        {
            var dbModel = await _paymentRepository.GetPaymentInfoModel(referenceId);
            var model = Mapper.Map<PaymentInfoEntity, PaymentInfoModel>(dbModel);

            return model;
        }

        public async Task<int> SetPaymentInfo(PaymentInfoModel paymentInfoModel)
        {
            var form = Mapper.Map<PaymentInfoModel, PaymentInfoEntity>(paymentInfoModel);
            var ret = await _paymentRepository.SavePaymentInfo(form);

            return ret;
        }

        public async Task<PaymentTransactionModel> GetPaymentTransaction(string referenceId)
        {
            var dbModel = await _paymentRepository.GetPaymentTransaction(referenceId);
            if (dbModel == null)
                return new PaymentTransactionModel { Error = "Invalid reference id!" };
            return Mapper.Map<PaymentTransactionModel>(dbModel);
        }

         
        public async Task<int> SavePaymentTransactionInfo(PaymentTransactionModel paymentTransactionModel)
        {
            _logger.LogInformation($"Posted PaymentTransactionModel: {JsonSerializer.Serialize(paymentTransactionModel)}");
            var form = Mapper.Map<PaymentTransactionEntity>(paymentTransactionModel);
            int ret = 0;
      
                ret = await _paymentRepository.SavePaymentTransaction(form);

                return ret;

            }

        public async Task<int> SetPaymentTransaction(PaymentTransactionModel paymentTransactionModel, bool newRecord = false)
        {
            _logger.LogInformation($"Posted PaymentTransactionModel: {JsonSerializer.Serialize(paymentTransactionModel)}");
            var form = Mapper.Map<PaymentTransactionEntity>(paymentTransactionModel);
            int ret = 0;
            LockerBookingEntity existingBooking = null;
            if (newRecord)
            {
                ret = await _paymentRepository.SavePaymentTransaction(form);
            }
            else
            {
                var existingPaymentTransaction = await _paymentRepository.GetPaymentTransaction(paymentTransactionModel.TransactionId);
                if (existingPaymentTransaction == null)
                    ret = await _paymentRepository.SavePaymentTransaction(form);
                else if (existingPaymentTransaction.InternalType
                    == PaymentInternalType.CancelingBooking ||
                    existingPaymentTransaction.InternalType
                    == PaymentInternalType.ExtendingBooking)
                {
                    existingPaymentTransaction.Status = paymentTransactionModel.Status;
                    existingPaymentTransaction.Type = paymentTransactionModel.Type;
                    if (paymentTransactionModel.Status == PayamayaStatus.PaymentSuccess)
                    {
                        existingPaymentTransaction.InternalStatus = PaymentInternalStatus.Paid;
                        if (existingPaymentTransaction.LockerTransactionsId.HasValue)
                        {
                            existingBooking = await lockerRepository.GetLockerBooking(existingPaymentTransaction.LockerTransactionsId.Value);
                            if (existingBooking != null)
                                existingPaymentTransaction.NewStoragePeriodEndDate = existingBooking.NewStoragePeriodEndDate;
                        }

                    }

                    ret = await _paymentRepository.SavePaymentTransaction(existingPaymentTransaction);
                    _logger.LogInformation($"SavePaymentTransaction Response: {ret}");
                    //Sending Notification
                    if (ret > 0
                        && existingBooking != null
                        && existingBooking.NewStoragePeriodEndDate.HasValue
                        && existingPaymentTransaction != null
                        && existingPaymentTransaction.InternalType == PaymentInternalType.ExtendingBooking
                        && existingPaymentTransaction.InternalStatus == PaymentInternalStatus.Paid)
                    {
                        var userDetail = await _userService.GetUserByUserKeyId(existingBooking.UserKeyId);
                        var refCode = existingPaymentTransaction.TransactionId;
                        var subject = "Your Booking has been extended";
                        //Message For SMS
                        var smsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerBookingExtendedShortMessage).MappedResponseValidityModel().Message;
                        smsMsg = smsMsg.Replace(MessageParameters.StartDate, existingBooking.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                        smsMsg = smsMsg.Replace(MessageParameters.EndDate, existingBooking.NewStoragePeriodEndDate.Value.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                        smsMsg = smsMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                        smsMsg = smsMsg.Replace(MessageParameters.PlanType, existingBooking.AccessPlan == 2 ? "multi access" : "single access");
                        //Message For Email
                        var emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerBookingExtendedLongMessage).MappedResponseValidityModel().Message;
                        emailMsg = emailMsg.Replace(MessageParameters.UserName, userDetail.FirstName);
                        emailMsg = emailMsg.Replace(MessageParameters.PlanType, existingBooking.AccessPlan==2 ? "multi access" : "single access");
                        emailMsg = emailMsg.Replace(MessageParameters.StartDate, existingBooking.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                        emailMsg = emailMsg.Replace(MessageParameters.EndDate, existingBooking.NewStoragePeriodEndDate.Value.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                        emailMsg = emailMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                        var notificationType = NotificationType.ForBookingExtentionSuccessNotification.ToString();
                        //SMS
                        if (!string.IsNullOrEmpty(smsMsg))
                        {
                            var isSent = await _httpService.SendSMS(userDetail.PhoneNumber, smsMsg, refCode);
                            if (isSent)
                                _logger.LogInformation($"Success sending of SMS for {notificationType} having {refCode} to user with Phone Number " + userDetail.PhoneNumber);
                            else
                                _logger.LogError($"Failed sending of SMS for {notificationType} having {refCode} to user with Phone Number " + userDetail.PhoneNumber);
                        }
                        //EMAIL
                        if (!string.IsNullOrEmpty(emailMsg))
                        {

                            var isSent = await _notificationService.SendSmtpEmailAsync(new EmailModel
                            {
                                Subject = subject,
                                Message = emailMsg,
                                To = userDetail.Email,
                                Type = MessageType.HtmlContent
                            });

                            if (isSent)
                                _logger.LogInformation($"Success sending of email for {notificationType} having {refCode} to user with email " + userDetail.Email);
                            else
                                _logger.LogError($"Failed sending of email for {notificationType} having {refCode} to user with email " + userDetail.Email);

                        }
                        

 
                         var   storagePeriodEnd = ((DateTime)existingBooking.NewStoragePeriodEndDate).ToUniversalTime();
                            //Scheduled job for notifying user before an hour of subscription expiration
                 
                            JobScheduler.Schedule<IUserService>(
                       (s) => s.UserExpiredBookingNotificaiton((int)existingPaymentTransaction.LockerTransactionsId),
                       storagePeriodEnd);
 

                    }
                }

            }
            return ret;
        }
    }
}
