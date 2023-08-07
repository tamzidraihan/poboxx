using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Models.Email;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Notification;
using SmartBox.Business.Services.Service.OTP;
using SmartBox.Business.Services.Service.User;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.Cabinet;
using SmartBox.Infrastructure.Data.Repository.Locker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Services.Service.LogIn;
using static SmartBox.Business.Shared.GlobalConstants;
using static SmartBox.Business.Shared.GlobalEnums;
using SmartBox.Business.Services.Service.Cabinet;
using NPOI.SS.Formula.Functions;
using System.Web.WebPages;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.User;
using SmartBox.Infrastructure.Data.Repository.Pricing;
using SmartBox.Business.Services.Service.CompanyUser;
using SmartBox.Infrastructure.Data.Repository.CompanyUser;
using Dapper;
using System.Data;
using SmartBox.Business.Services.Service.HTTPService;
using SmartBox.Infrastructure.Data.Repository.User;
using SmartBox.Business.Services.Service.Payment;
using SmartBox.Business.Core.Models.Payment;
using Microsoft.Extensions.Options;
using static Slapper.AutoMapper;
using NPOI.POIFS.Properties;

namespace SmartBox.Business.Services.Service.Locker
{
    public class LockerService : BaseMessageService<ILockerService>, ILockerService
    {
        private readonly ILockerRepository _lockerRepository;
        private readonly IOTPService _iOTPService;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly ILogger<LockerService> _logger;
        private readonly ICabinetService _cabinetService;
        private readonly IPriceAndChargingRepository priceAndChargingRepository;
        private readonly ICompanyUserRepository companyUserRepository;
        private readonly IHttpService _httpService;
        private readonly IUserSubscriptionRepository userSubscriptionRepository;
        private readonly IPaymentService paymentService;
        private readonly GlobalConfigurations globalConfigurations;
        public LockerService(IPaymentService paymentService, IAppMessageService appMessageService, IMapper mapper, ILockerRepository lockerRepository,
                            ICabinetRepository cabinetRepository, INotificationService notificationService,
                            IOTPService iOTPService, IUserService userService,
                            ICabinetService cabinetService,
                            IOptions<GlobalConfigurations> globalConfigurations,
                            IUserSubscriptionRepository userSubscriptionRepository,
                            ICompanyUserRepository companyUserRepository,
                            IPriceAndChargingRepository priceAndChargingRepository,
                            IHttpService httpService,
        ILogger<LockerService> logger) : base(appMessageService, mapper)
        {
            _lockerRepository = lockerRepository;
            _notificationService = notificationService;
            _iOTPService = iOTPService;
            _userService = userService;
            this.companyUserRepository = companyUserRepository;
            _logger = logger;
            this.priceAndChargingRepository = priceAndChargingRepository;
            _cabinetService = cabinetService;
            this.globalConfigurations = globalConfigurations.Value;
            _httpService = httpService;
            this.paymentService = paymentService;
            this.userSubscriptionRepository = userSubscriptionRepository;
        }

        async Task<ResponseValidityModel> ValidateLocker(LockerDetailModel lockerDetailModel, bool isInsert)
        {
            var model = new ResponseValidityModel();

            if (lockerDetailModel.CompanyId == 0)
                model.MessagesList.Add(nameof(LockerDetailModel.CompanyId));

            if (lockerDetailModel.LockerTypeId == 0)
                model.MessagesList.Add(nameof(LockerDetailModel.LockerTypeId));

            if (lockerDetailModel.CabinetId == 0)
                model.MessagesList.Add(nameof(LockerDetailModel.CabinetId));

            if (!lockerDetailModel.LockerNumber.HasText())
                model.MessagesList.Add(nameof(LockerDetailModel.LockerNumber));

            if (!lockerDetailModel.OpenCommand.HasText())
                model.MessagesList.Add(nameof(LockerDetailModel.OpenCommand));

            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired).MappedResponseValidityModel(messageList: model.MessagesList);
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, model.MessagesList.Count > 1 ? Shared.SharedServices.SetVerbMessage(true) : Shared.SharedServices.SetVerbMessage(false));
                return model;
            }

            var cabinet = (await _cabinetService.GetCabinet(lockerDetailModel.CabinetId)).FirstOrDefault();
            var locker = await _lockerRepository.GetLockerDetail(cabinetId: lockerDetailModel.CabinetId, lockerNumber: lockerDetailModel.LockerNumber);
            var lockerType = await _cabinetService.GetLockerType(lockerDetailModel.LockerTypeId);

            if (!lockerType.Any())
            {
                model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(MessageParameters.Field, "LockerType Id");
                return model;
            }

            if (cabinet == null)
            {
                model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(MessageParameters.Field, "Cabinet Id");
                return model;
            }

            if (isInsert)
            {
                var totalLocker = (await _lockerRepository.GetLockerDetail(cabinetId: lockerDetailModel.CabinetId)).Count;
                if (locker.Any())
                {
                    model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(MessageParameters.Field, "Locker Number");
                    return model;
                }

                if (totalLocker + 1 > cabinet.NumberOfLocker)
                {
                    model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.ExceedNumberOfLocker).MappedResponseValidityModel();
                    return model;
                }
            }
            return model;
        }
        public async Task<ResponseValidityModel> ExtendLockerBooking(ExtendLockerBookingModel model, string UserKeyId)
        {

            var response = ValidateExtendLockerBooking(model);
            if (response.MessageReturnNumber > 0)
                return AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            var bookingDetail = await _lockerRepository.GetLockerBooking(model.LockerTransactionsId);
            if (bookingDetail == null || bookingDetail.StoragePeriodStart >= model.NewStorageEndDate)
                return AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            var existingBookingsCount = await _lockerRepository.ActiveBookingsCount(bookingDetail.LockerDetailId, bookingDetail.StoragePeriodStart, model.NewStorageEndDate, bookingDetail.LockerTransactionsId);
            if (existingBookingsCount > 0)
            {
                var res = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.AlreadyBooked).MappedResponseValidityModel();
                res.Message = res.Message.Replace(MessageParameters.StartDate, bookingDetail.StoragePeriodStart.ToString());
                res.Message = res.Message.Replace(MessageParameters.EndDate, model.NewStorageEndDate.ToString());
                return res;
            }

            var ret = await _lockerRepository.ExtendLockerBooking(model, bookingDetail, UserKeyId);
            if (ret > 0)
            {
                await SendBookingNotification(bookingDetail.LockerTransactionsId, NotificationType.ForBookingExtentionSuccessNotification);
                //User Notification to notify user before expiration date
                ScheduleUserNotifications(bookingDetail.LockerTransactionsId, model.NewStorageEndDate);
                response = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.RecordUpdated).MappedResponseValidityModel();
            }
            else
                response = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            return response;
        }
        ResponseValidityModel ValidateExtendLockerBooking(ExtendLockerBookingModel model)
        {
            var response = new ResponseValidityModel();
            if (model.LockerTransactionsId < 1 || model.NewTotalPrice < 1)
                response.MessageReturnNumber = 1;

            return response;
        }
        public ResponseValidityModel ValidateLockerBooking(ExtendLockerBookingModel model, string UserKeyId)
        {
            var response = new ResponseValidityModel();
            if (model.StartStorageDateTime > DateTime.Now)
                return AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingExpired).MappedResponseValidityModel();
            else
                return response;

        }
        bool IsValidBooking(LockerMobileBookingModel model, bool isSubscribed)
        {
            if (isSubscribed)
            {
                if (model.LockerDetailId < 1 || model.LockerTypeId < 1
                    || model.CabinetLocationId < 1
                     || string.IsNullOrEmpty(model.PaymentReference)
                    || model.StoragePeriodStart > model.StoragePeriodEnd
                    || model.StoragePeriodStart < DateTime.Now
                    || model.StoragePeriodEnd < DateTime.Now)
                    return false;
            }
            else
            {
                if (model.LockerDetailId < 1 || model.LockerTypeId < 1
                    || model.CabinetLocationId < 1
                    || string.IsNullOrEmpty(model.ReceiverName)
                     || string.IsNullOrEmpty(model.PaymentReference)
                    || model.StoragePeriodStart > model.StoragePeriodEnd)
                    return false;
            }

            return true;
        }
        bool IsValidBooking(LockerManualBookingModel model, bool isSubscribed)
        {

            if (isSubscribed)
            {
                if (model.LockerDetailId < 1 || model.LockerTypeId < 1
                    || model.CabinetLocationId < 1
                     || string.IsNullOrEmpty(model.PaymentReference)
                    || model.StoragePeriodStart > model.StoragePeriodEnd
                    || model.StoragePeriodStart < DateTime.Now
                    || model.StoragePeriodEnd < DateTime.Now)
                    return false;
            }
            else
            {
                if (model.LockerDetailId < 1 || model.LockerTypeId < 1
                    || model.CabinetLocationId < 1
                    || string.IsNullOrEmpty(model.ReceiverName)
                     || string.IsNullOrEmpty(model.PaymentReference)
                    || model.StoragePeriodStart > model.StoragePeriodEnd)
                    return false;
            }
            return true;
        }


        public async Task<List<LockerViewModel>> GetLocker(int? lockerId, int? cabinetId = null, int? companyId = null)
        {
            var dbModel = await _lockerRepository.GetLockerDetail(lockerId, cabinetId: cabinetId, companyId: companyId);
            var lockerDetailModelList = new List<LockerViewModel>();
            if (dbModel != null)
            {
              
                LockerStatus lockerStatus = new LockerStatus();
                foreach (var m in dbModel)
                {
                    var lockerDetailModel = new LockerViewModel();
                    if (m.BookingStatus == null)
                        lockerStatus = LockerStatus.Empty;
                    else if (m.BookingStatus == 1 || m.BookingStatus == 2)
                        lockerStatus = LockerStatus.Book;
                    else
                        lockerStatus = LockerStatus.Empty;


                    if (m.IsAvailable == false)
                        lockerStatus = LockerStatus.UnderRepair;

                    lockerDetailModel.CabinetLocationAddress = m.CabinetLocationAddress;
                    lockerDetailModel.CabinetLocationDescription = m.CabinetLocationDescription;
                    lockerDetailModel.CabinetId = m.CabinetId;
                    lockerDetailModel.CompanyId = m.CompanyId;
                    lockerDetailModel.BoardNumber = m.BoardNumber;
                    lockerDetailModel.OpenCommand = m.OpenCommand;
                    lockerDetailModel.LockerNumber = m.LockerNumber;
                    lockerDetailModel.GetStatusCommand = m.GetStatusCommand;
                    lockerDetailModel.LockerDetailId = m.LockerDetailId;
                    lockerDetailModel.PositionId = m.PositionId;
                    lockerDetailModel.Size = m.Size;
                    lockerDetailModel.LockerTypeDescription = m.LockerTypeDescription;
                    lockerDetailModel.LockerTypeId = m.LockerTypeId;
                    lockerDetailModel.IsAvailable = m.IsAvailable;
                    lockerDetailModel.LockerStatus = lockerStatus;
                    lockerDetailModel.IsDeletedLockerType = m.IsDeletedLockerType;
                   

                    lockerDetailModelList.Add (lockerDetailModel);
                }

  
                return lockerDetailModelList;
            }

            return new List<LockerViewModel>();
        }

        public async Task<List<LockerDetailModel>> GetActiveLocker(int? lockerDetailId, int? companyId = null)
        {
            var dbModel = await _lockerRepository.GetActiveLocker(lockerDetailId, companyId: companyId);
            if (dbModel != null)
            {
                var model = Mapper.Map<List<LockerDetailModel>>(dbModel);
                return model;
            }

            return new List<LockerDetailModel>();
        }
        public async Task<List<LockerDetailModel>> GetUnavailableLockers(DateTime? startDate = null, DateTime? endDate = null)
        {
            var model = new List<LockerDetailModel>();
            var dbModel = await _lockerRepository.GetUnavailableLockers(startDate, endDate);
            if (dbModel != null && dbModel.Count > 0)
            {
                var groupBy = dbModel.GroupBy(s => s.LockerDetailId)
                    .Select(s => new { key = s.Key, lst = s.ToList() }).ToList();
                foreach (var item in groupBy)
                {
                    var currentLocker = item.lst.First();
                    var lockerDetail = Mapper.Map<LockerDetailModel>(currentLocker);
                    lockerDetail.Bookings = Mapper.Map<List<LockerDetailBooking>>(item.lst);
                    model.Add(lockerDetail);
                }
                return model;
            }

            return model;
        }
        public async Task<List<LockerDetailStatusModel>> GetActiveLockerStatus(int? lockerDetailId, int? companyId = null, int? cabinetId = null)
        {
            var dbModel = await _lockerRepository.GetActiveLockerStatus(lockerDetailId, companyId: companyId, cabinetId: cabinetId);
            if (dbModel.Count > 0)
            {
                var model = new List<LockerDetailStatusModel>();
                bool isAvailable;
                foreach (var item in dbModel)
                {
                    isAvailable = true;
                    var lockerStatus = LockerStatus.Empty;

                    //isAvailable
                    //True - if no booking by the time the api is requested and the locker isAvailable value on the locker detail is true
                    //False - if there's booking and and value for isAvailable is false in lockerdetail table


                    //lockerStatus
                    //LockerStatus: Book - if there's a current booking
                    //LockerStatus: Empty - if no booking and available
                    //LockerStatus: Under Repair - if the isAvailable value in the locker detail is false
                    if (item.BookingStatus == null)
                        lockerStatus = LockerStatus.Empty;
                    else if (item.BookingStatus == 1 || item.BookingStatus == 2)
                        lockerStatus = LockerStatus.Book;
                    else
                        lockerStatus = LockerStatus.Empty;



                    if (item.IsAvailable == false)
                        lockerStatus = LockerStatus.UnderRepair;

                    model.Add(new LockerDetailStatusModel
                    {
                        BoardNumber = item.BoardNumber,
                        CabinetId = item.CabinetId,
                        GetStatusCommand = item.GetStatusCommand,
                        LockerDetailId = item.LockerDetailId,
                        LockerNumber = item.LockerNumber,
                        LockerTypeId = item.LockerTypeId,
                        OpenCommand = item.OpenCommand,
                        PositionId = item.PositionId,
                        CompanyId = item.CompanyId,
                        IsAvailable = isAvailable,
                        LockerStatus = lockerStatus
                    });
                }
                return model;
            }

            return new List<LockerDetailStatusModel>();
        }

        public async Task<ResponseValidityModel> SaveLocker(LockerDetailModel lockerDetailModel)
        {
            bool isInsert = true;

            if (lockerDetailModel.LockerDetailId != 0)
                isInsert = false;

            var model = await ValidateLocker(lockerDetailModel, isInsert);
            var entity = Mapper.Map<LockerDetailModel, LockerDetailEntity>(lockerDetailModel);

            if (model.MessageReturnNumber >= 0)
            {
                var ret = await _lockerRepository.SaveLocker(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }

            return model;
        }

        public async Task<List<AvailableLockerModel>> GetAvailableLocker(int? cabinetLocationId, int? lockerTypeId, int? selectedMonth, int? selectedYear,
                                                                         DateTime? startDate, DateTime? endDate,
                                                                         bool isOrderByLockerDetailId, int? cabinetId, int? currentPage,
                                                                         int? pageSize, int? positionId = null)
        {
            var strDate = endDate.ToString();
            var dbModel = await _lockerRepository.GetAvailableLocker(cabinetLocationId, lockerTypeId, selectedMonth, selectedYear,
                                                                     startDate, endDate, isOrderByLockerDetailId, cabinetId,
                                                                     currentPage, pageSize, positionId);
            if (dbModel == null || dbModel.Count < 1)
                return new List<AvailableLockerModel>();

            return ProcessAvailableLockersPrices(dbModel, startDate, endDate);
        }
        public async Task<List<UpdatedAvailableLockerModel>> GetUpdatedAvailableLocker(int? cabinetLocationId, int? lockerTypeId,
                                                                         DateTime? startDate, DateTime? endDate,
                                                                         bool isOrderByLockerDetailId, int? cabinetId, int? currentPage,
                                                                         int? pageSize, int? positionId = null)
        {
            var dbModel = await _lockerRepository.GetUpdatedAvailableLocker(cabinetLocationId, lockerTypeId,
                                                                     startDate, endDate, isOrderByLockerDetailId, cabinetId,
                                                                     currentPage, pageSize, positionId);
            if (dbModel == null || dbModel.Count < 1)
                return new List<UpdatedAvailableLockerModel>();

            return ProcessAvailableLockersPrices(dbModel, startDate, endDate);
        }
        public async Task<UpdatedAvailableLockerModel> GetBookingUpdatedPrice(int lockerTransactionId, int lockerDetailId,
                                                                        DateTime startDate, DateTime endDate)
        {
            if (lockerTransactionId < 1 || lockerDetailId < 1)
                return new UpdatedAvailableLockerModel();
            var dbModel = await _lockerRepository.GetBookingUpdatedPrice(lockerTransactionId, lockerDetailId, endDate);
            if (dbModel == null)
                return new UpdatedAvailableLockerModel();
            var model = Mapper.Map<UpdatedAvailableLockerModel>(dbModel);



            model.Price = CalculatePriceMatrix(model.StoragePrice, model.OverstayCharge, model.OverstayPeriod, model.PricingType, dbModel.StoragePeriodEnd, endDate);
            model.OverstayCharge = (model.StoragePrice * (model.OverstayCharge / 100));
            model.MultiAccessStoragePrice = CalculatePriceMatrix(model.MultiAccessStoragePrice, model.OverstayCharge, model.OverstayPeriod, model.PricingType, dbModel.StoragePeriodEnd, endDate);

            return model;
        }


        public async Task<List<BookingTransactionsViewModel>> GetBookingTransactions(DateTime? startDate, DateTime?
                                                                                endDate, int? companyId = null, int? currentPage = null, int? pageSize = null, BookingTransactionStatus? bookingStatus = null)
        {
             var bookingTransactions = await _lockerRepository.GetBookingTransactions(startDate, endDate, companyId, currentPage, pageSize, bookingStatus);
            var modelList = new List<BookingTransactionsViewModel>();
            foreach (var l in bookingTransactions)
            {
                if (DateTime.Now > l.StoragePeriodEnd && (l.BookingStatus == 2 || l.BookingStatus == 1))
                {
                   
                  await  _lockerRepository.UpdateBookingStatus(l.LockerTransactionsId,7);
                    l.BookingStatus = 7;                       
                }
                TimeSpan duration = l.StoragePeriodEnd - l.StoragePeriodStart ;
                int difference = (int)duration.TotalHours;

                if (difference >= 24)
                    difference = duration.Days;


                l.TotalStorageTime = difference;
                modelList.Add(l);
            }

            return modelList;

        }

        public async Task<List<BookingTransactionsViewModel>> GetUserBookingTransactions(string userKeyId, DateTime? startDate, DateTime?
                                                                          endDate, int? companyId = null,
                                                                        int? currentPage = null, int? pageSize = null,
                                                                        BookingTransactionStatus? bookingStatus = null)
        {
            var bookingTransactions = await _lockerRepository.GetUserBookingTransactions(userKeyId, startDate, endDate, companyId, currentPage, pageSize, bookingStatus);
            var modelList = new List<BookingTransactionsViewModel>();

            foreach (var l in bookingTransactions)
            {
                if (DateTime.Now > l.StoragePeriodEnd && (l.BookingStatus == 2 || l.BookingStatus==1))
                {

                    await _lockerRepository.UpdateBookingStatus(l.LockerTransactionsId, 7);
                    l.BookingStatus = 7;
                }

                modelList.Add(l);
            }

            return modelList;

        }

        public async Task<AvailableLockerListModel> GetAvailableLockerList(int? cabinetLocationId, int? lockerTypeId, int? selectedMonth, int? selectedYear, DateTime? startDate, DateTime? endDate, bool isOrderByLockerDetailId, int? cabinetId, int? currentPage, int? pageSize)
        {
            var dbModel = await _lockerRepository.GetAvailableLocker(cabinetLocationId, lockerTypeId, selectedMonth, selectedYear,
                                                                    startDate, endDate, isOrderByLockerDetailId, cabinetId,
                                                                    currentPage, pageSize);
            var model = new AvailableLockerListModel();

            if (dbModel == null || dbModel.Count < 1)
                return new AvailableLockerListModel();

            var list = ProcessAvailableLockersPrices(dbModel, startDate, endDate); ;
            model.TotalRecord = dbModel[0].TotalRecordCount;
            model.PageSize = (int)pageSize;
            model.CurrentPage = (int)currentPage;
            model.Collection = list;

            return model;

        }

        public async Task<List<AvailableLockerModel>> GetAvailableDistinctLocker(int? cabinetLocationId, int? lockerTypeId, int? selectedMonth, int? selectedYear,
                                                                  DateTime? startDate, DateTime? endDate,
                                                                  bool isOrderByLockerDetailId, int? cabinetId,
                                                                  int? currentPage, int? pageSize)
        {
            var dbModel = await _lockerRepository.GetAvailableLocker(cabinetLocationId, lockerTypeId, selectedMonth, selectedYear,
                                                                     startDate, endDate, isOrderByLockerDetailId, cabinetId,
                                                                     currentPage, pageSize);
            if (dbModel == null || dbModel.Count < 1)
                return new List<AvailableLockerModel>();

            var distinct = dbModel.GroupBy(p => p.LockerDetailId).Select(g => g.First()).ToList();

            //var q = distinct.GroupBy(x => new { x.LockerTypeId, x.LockerTypeDescription, x.Size, x.Price })
            //        .Select(g => new { g.Key, Count = g.Count() });
            return ProcessAvailableLockersPrices(distinct, startDate, endDate);

        }
        private List<AvailableLockerModel> ProcessAvailableLockersPrices(List<AvailableLockerEntity> lst, DateTime? startDate, DateTime? endDate)
        {
            var model = Mapper.Map<List<AvailableLockerModel>>(lst);
            model = model.Select(o =>
            {
                o.Price = CalculatePriceMatrix(o.StoragePrice, o.OverstayCharge, o.OverstayPeriod, o.PricingType, startDate, endDate);
                o.OverstayCharge = (o.StoragePrice * (o.OverstayCharge / 100));
                return o;
            }).ToList();
            return model;
        }
        private List<UpdatedAvailableLockerModel> ProcessAvailableLockersPrices(List<UpdatedAvailableLockerEntity> lst, DateTime? startDate, DateTime? endDate)
        {
            var model = Mapper.Map<List<UpdatedAvailableLockerModel>>(lst);
            model = model.Select(o =>
            {
                o.Price = CalculatePriceMatrix(o.StoragePrice, o.OverstayCharge, o.OverstayPeriod, o.PricingType, startDate, endDate);
                o.OverstayChargeValue = o.OverstayCharge;
                o.OverstayCharge = (o.Price * (o.OverstayCharge / 100));
                
                o.MultiAccessStoragePrice = CalculatePriceMatrix(o.MultiAccessStoragePrice, o.OverstayCharge, o.OverstayPeriod, o.PricingType, startDate, endDate);
                
                return o;
            }).ToList();
            return model;
        }
        decimal CalculatePriceMatrix(decimal storagePrice, decimal overstayCharge, int defaultOverstayPeriod,
            string type, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                if (type != null && type.ToLowerInvariant() == "daily")
                {
                    var noOfDays = (int)Math.Ceiling(endDate.Value.Subtract(startDate.Value).TotalDays);
                    if (noOfDays > 0)
                        return (storagePrice * noOfDays);

                    else
                        return storagePrice;
                }
                else if (type != null && type.ToLowerInvariant() == "hourly")
                {
                    var noOfHours = (int)Math.Ceiling(endDate.Value.Subtract(startDate.Value).TotalHours);
                    if (noOfHours > 0)
                        return storagePrice * noOfHours;
                    else
                        return storagePrice;
                }
                else if (type != null && type.ToLowerInvariant() == "monthly")
                {
                    var noOfDays = Math.Ceiling(endDate.Value.Subtract(startDate.Value).TotalDays);
                    var noOfMonths = (int)Math.Round(noOfDays / 30);

                    if (noOfMonths > 0)
                        return storagePrice * noOfMonths;
                    else
                        return storagePrice;
                }

                else return storagePrice;
            }
            else return storagePrice;

        }

        decimal CalculateOverStayCharge(decimal storagePrice, decimal overstayCharge,
           string type, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                if (type != null && type.ToLowerInvariant() == "daily")
                {
                    var noOfDays = (int)endDate.Value.Subtract(startDate.Value).TotalDays;
                    return noOfDays * (overstayCharge / 100) * storagePrice;
                }
                else if (type != null && type.ToLowerInvariant() == "hourly")
                {
                    var noOfHours = (int)endDate.Value.Subtract(startDate.Value).TotalHours;
                    return noOfHours * (overstayCharge / 100) * storagePrice;
                }
                else if (type != null && type.ToLowerInvariant() == "monthyly")
                {
                    var noOfMonths = (int)(endDate.Value.Subtract(startDate.Value).TotalDays / 30);
                    return noOfMonths * (overstayCharge / 100) * storagePrice;
                }

                else return storagePrice;
            }
            else return storagePrice;

        }
        public async Task<List<OTPModel>> SaveBookingLocker(List<LockerMobileBookingModel> lockerMobileBookingModelM, string userKeyId)
        {
            var modelList = new List<OTPModel>();
            var existingList = new List<LockerMobileBookingModel>();
            if (string.IsNullOrEmpty(userKeyId)) return modelList;
            bool isSubscribed = false;
            foreach (LockerMobileBookingModel lockerMobileBookingModel in lockerMobileBookingModelM)
            {
                var userSubscription = await userSubscriptionRepository.GetUserSubscription(userKeyId, lockerMobileBookingModel.LockerDetailId, lockerMobileBookingModel.StoragePeriodEnd);
                isSubscribed = userSubscription.Count > 0;
                if (isSubscribed)
                    lockerMobileBookingModel.StoragePeriodEnd = userSubscription.First().ExpiryDate;

                var isValid = IsValidBooking(lockerMobileBookingModel, isSubscribed);
                if (!isValid)
                {
                    modelList.Add(new OTPModel
                    {
                        ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidInput).MappedResponseValidityModel()
                    });
                    continue;
                }
                var existingBookings =   _lockerRepository.ActiveBookingsCount(lockerMobileBookingModel.LockerDetailId, lockerMobileBookingModel.StoragePeriodStart,
                   lockerMobileBookingModel.StoragePeriodEnd).Result;
                if (existingBookings > 0)
                {
                    var model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.AlreadyBooked).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(MessageParameters.StartDate, lockerMobileBookingModel.StoragePeriodStart.ToString());
                    model.Message = model.Message.Replace(MessageParameters.EndDate, lockerMobileBookingModel.StoragePeriodEnd.ToString());
                    modelList.Add(new OTPModel
                    {
                        ValidityModel = model
                    });

                    //send notification
                  /*  if (!existingList.Any(s=>s.PaymentReference == lockerMobileBookingModel.PaymentReference))
                                 existingList.Add(lockerMobileBookingModel);
                  */
                    continue;
                }
                var entity = Mapper.Map<LockerMobileBookingModel, LockerBookingEntity>(lockerMobileBookingModel);
                entity.UserKeyId = userKeyId;
                modelList.Add(await ProcessBooking(entity, isSubscribed));
            }
            if (modelList.Any(s => s.LockerTransactionsId > 0))
                await SendMultiBookingDropOffEmail(modelList.Where(s => s.LockerTransactionsId > 0).Select(s => s.LockerTransactionsId).ToList());
            
            if (existingList.Count >1)
            {
                foreach (var lockerMobileBookingModel in existingList)
                {
                    SendBookingCancellationNotification(lockerMobileBookingModel).RunSynchronously();
                    var lockerDetails = await _lockerRepository.GetLockerDetail(lockerMobileBookingModel.LockerDetailId);
                    var detail = lockerDetails.First();


                    if (detail == null)
                        continue;
                    else
                    {
                        _logger.LogInformation($"Booking Cancelled/For Refund - Payment Ref Code:{lockerMobileBookingModel.PaymentReference} : Locker No. {detail.LockerNumber} at {detail.CabinetLocationDescription}, {detail.CabinetLocationAddress} " +
                            $"for {lockerMobileBookingModel.ReceiverName} with amount {lockerMobileBookingModel.TotalPrice}");
                    }
                }
            }
            return modelList;
        }

        public async Task<List<OTPModel>> SaveLockerBooking(List<LockerManualBookingModel> lockerManualBookingModelM)
        {
            var modelList = new List<OTPModel>();
            bool isSubscribed = false;
            foreach (LockerManualBookingModel lockerManualBookingModel in lockerManualBookingModelM)
            {
                if (string.IsNullOrEmpty(lockerManualBookingModel.UserKeyId)) continue;
                var userSubscription = await userSubscriptionRepository.GetUserSubscription(lockerManualBookingModel.UserKeyId, lockerManualBookingModel.LockerDetailId, lockerManualBookingModel.StoragePeriodEnd);
                isSubscribed = userSubscription.Count > 0;
                if (isSubscribed)
                    lockerManualBookingModel.StoragePeriodEnd = userSubscription.First().ExpiryDate;
                var isValid = IsValidBooking(lockerManualBookingModel, isSubscribed);
                if (!isValid)
                {
                    modelList.Add(new OTPModel
                    {
                        ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidInput).MappedResponseValidityModel()
                    });
                    continue;
                }
                var existingBookings = await _lockerRepository.ActiveBookingsCount(lockerManualBookingModel.LockerDetailId, lockerManualBookingModel.StoragePeriodStart,
                   lockerManualBookingModel.StoragePeriodEnd);
                if (existingBookings > 0)
                {
                    var model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.AlreadyBooked).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(MessageParameters.StartDate, lockerManualBookingModel.StoragePeriodStart.ToString());
                    model.Message = model.Message.Replace(MessageParameters.EndDate, lockerManualBookingModel.StoragePeriodEnd.ToString());
                    modelList.Add(new OTPModel
                    {
                        ValidityModel = model
                    });
                    continue;
                }
                var entity = Mapper.Map<LockerManualBookingModel, LockerBookingEntity>(lockerManualBookingModel);
                modelList.Add(await ProcessBooking(entity, isSubscribed));
            }
            if (modelList.Any(s => s.LockerTransactionsId > 0))
                await SendMultiBookingDropOffEmail(modelList.Where(s => s.LockerTransactionsId > 0).Select(s => s.LockerTransactionsId).ToList());
            return modelList;
        }
        private async Task<OTPModel> ProcessBooking(LockerBookingEntity entity, bool isSubscribed)
        {
            var model = new OTPModel();
            var user = await _userService.GetUserByUserKeyId(entity.UserKeyId);
            if (isSubscribed)
            {
                entity.BookingStatus = 2;
                entity.PickUpCode = await _iOTPService.GenerateOTP();
                entity.PickUpQRCode = string.Concat(entity.LockerDetailId, entity.PickUpCode);
                entity.IsSubscriptionBooking = true;
            }
            else
            {
                entity.BookingStatus = 1;
                entity.DropOffCode = await _iOTPService.GenerateOTP();
                entity.DropOffQRCode = string.Concat(entity.LockerDetailId, entity.DropOffCode);
                entity.IsSubscriptionBooking = false;
            }
            var ret = await _lockerRepository.SaveLockerBooking(entity);

            if (ret > 0)
            {
                if (!isSubscribed)
                {
                    ScheduleUserNotifications(ret, entity.StoragePeriodEnd);
                    await SendBookingNotification(ret, NotificationType.ForDropOffPin);
                }
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.RecordAdded).MappedResponseValidityModel();
                model.OTP = isSubscribed ? entity.PickUpCode : entity.DropOffCode;
                model.QRCode = isSubscribed ? entity.PickUpQRCode : entity.DropOffQRCode;
                model.BoardNumber = entity.BoardNumber;
                model.LockerNumber = entity.LockerNumber;
                model.LockerTransactionsId = ret;
            }
            else
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            return model;
        }
        public void ScheduleUserNotifications(int id, DateTime storagePeriodEnd)
        {

            _logger.LogInformation($"Schedule notification for lockerTransactionId {id} and StorageEnd {storagePeriodEnd}");
            storagePeriodEnd = storagePeriodEnd.ToUniversalTime();
            //Scheduled job for notifying user before an hour of subscription expiration
            JobScheduler.Schedule<IUserService>(
        (s) => s.UserLastHourSubscriptionNotification(id),
        storagePeriodEnd.AddHours(-1));

            JobScheduler.Schedule<IUserService>(
       (s) => s.UserExpiredBookingNotificaiton(id),
       storagePeriodEnd);


            //Scheduled job for notifying user before a week of subscription expiration
            if (DateTime.Now.AddDays(7) < storagePeriodEnd)
            {
                JobScheduler.Schedule<IUserService>(
           (s) => s.UserLastWeekSubscriptionNotification(id),
           storagePeriodEnd.AddDays(-7));
            }
        }
        public async Task<OTPModel> GenerateOTP(int lockerTransactionId, /*int cabinetId, int? companyId = null,*/ string userKeyId = null)
        {
            var entity = await _lockerRepository.GetLockerBooking(lockerTransactionId);
            var model = new OTPModel();

            if (entity == null /*|| cabinetId < 1*/)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.TransactionNotExisting).MappedResponseValidityModel();
                return model;
            }
            //var lockerDetails = await _lockerRepository.GetActiveLocker(lockerDetailId: entity.LockerDetailId, cabinetId: cabinetId);
            //if (lockerDetails.Count < 1)
            //{
            //    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.TransactionNotExisting).MappedResponseValidityModel();
            //    return model;
            //}
            //if (companyId.HasValue && lockerDetails.First().CompanyId != companyId)
            //{
            //    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.TransactionNotExisting).MappedResponseValidityModel();
            //    return model;
            //}
            if (string.IsNullOrEmpty(userKeyId))
                userKeyId = entity.UserKeyId;
            var userSubscriptions = await userSubscriptionRepository.GetUserSubscription(userKeyId, entity.LockerDetailId, entity.StoragePeriodEnd);
            bool isSubscribed = userSubscriptions.Count > 0;
            var userSubscription = userSubscriptions.FirstOrDefault();
            if ((entity.StoragePeriodEnd < DateTime.Now && !isSubscribed)
                || (userSubscription != null && userSubscription.ExpiryDate < DateTime.Now && isSubscribed))
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpired).MappedResponseValidityModel();
                return model;
            }
            var pin = await _iOTPService.GenerateOTP();
            var qrCode = string.Concat(entity.LockerDetailId, pin);

            if (entity.BookingStatus == 1) //drop-off to pick-up
            {
                var ret = await _lockerRepository.UpdatedOTP(BookingTransactionStatus.ForPickUp, pin, qrCode, entity.LockerTransactionsId);
                if (ret > 0)
                {
                    model.OTP = pin;
                    model.QRCode = qrCode;
                    model.LockerNumber = entity.LockerNumber;
                    model.LockerTransactionsId = entity.LockerTransactionsId;
                    model.BoardNumber = entity.BoardNumber;
                    model.GetStatusCommand = entity.GetStatusCommand;
                    await SendBookingNotification(entity.LockerTransactionsId, NotificationType.ForCollectPin);
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.NotificationSent).MappedResponseValidityModel();
                    return model;
                }
                else
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NoItemSave).MappedResponseValidityModel();
                    return model;
                }
            }
            else if (entity.BookingStatus == 2 && isSubscribed)
            {
                var ret = await _lockerRepository.UpdatedOTP(BookingTransactionStatus.ForPickUp, pin, qrCode, entity.LockerTransactionsId, isSubscribed);
                if (ret > 0)
                {
                    model.OTP = pin;
                    model.QRCode = qrCode;
                    model.LockerNumber = entity.LockerNumber;
                    model.LockerTransactionsId = entity.LockerTransactionsId;
                    model.BoardNumber = entity.BoardNumber;
                    model.GetStatusCommand = entity.GetStatusCommand;
                    model.OpenCommand = entity.OpenCommand;
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.NotificationSent).MappedResponseValidityModel();
                    return model;
                }
                else
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NoItemSave).MappedResponseValidityModel();
                    return model;
                }
            }
            else if (entity.BookingStatus == 2) 
            {
                int ret = 0;
                NotificationType notificationType = new  NotificationType();
                string type = string.Empty;

                if (entity.AccessPlan == 2)
                {//pick-up to dropPff multi access
                    ret = await _lockerRepository.UpdatedOTP(BookingTransactionStatus.ForDropOff, pin, qrCode, entity.LockerTransactionsId);
                    notificationType = NotificationType.ForDropOffPin;
                    type = "Collect";
                    
                }
                else
                {//pick-up to completed
                    ret = await _lockerRepository.UpdatedOTP(BookingTransactionStatus.Completed, pin, qrCode, entity.LockerTransactionsId);
                    notificationType = NotificationType.ForCollectSuccessNotification;
                    type = "Completed";

                }

                if (ret > 0)
                {
                    try
                    {
                        model.LockerNumber = entity.LockerNumber;
                        model.LockerTransactionsId = entity.LockerTransactionsId;
                        model.BoardNumber = entity.BoardNumber;
                        model.GetStatusCommand = entity.GetStatusCommand;
                        model.OpenCommand = entity.OpenCommand;
                        //model.ValidityModel.Message = model.ValidityModel.Message.Replace(GlobalConstants.MessageParameters.Name, entity?.ReceiverName);
                        //model.ValidityModel.Message = model.ValidityModel.Message.Replace(GlobalConstants.MessageParameters.Type, type);
                        //model.ValidityModel.Message = model.ValidityModel.Message.Replace(GlobalConstants.MessageParameters.LockerNumber, entity.LockerNumber);
                        await SendBookingNotification(entity.LockerTransactionsId, notificationType);
                        model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.NotificationSent).MappedResponseValidityModel();
                    }catch (Exception ex)
                    {
                        _logger.LogError("Error in generate otp:" + ex.Message);

                        model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
                     
                    }
                        return model;
                }
                else
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NoItemSave).MappedResponseValidityModel();
                    return model;
                }
            }
            else if (entity.BookingStatus == 4)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.CompletedTransaction).MappedResponseValidityModel();
                return model;
            }
            else
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.PassDeadline).MappedResponseValidityModel();
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.StartDate, entity.StoragePeriodStart.ToShortDateString());
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.EndDate, entity.StoragePeriodEnd.ToShortDateString());
                return model;
            }
        }

        public async Task<OTPModel> ValidateOTP(string OTPCode, int bookingStatus, int cabinetId, int? companyId = null)
        {
            var model = new OTPModel();

            if ((bookingStatus != 1 && bookingStatus != 2) || cabinetId < 1)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidCustomField).MappedResponseValidityModel();
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.Field, "Booking Status");
                return model;
            }
           
            var lockerWithDropOff = await _lockerRepository.GetDropOffOTP(OTPCode);
            if (lockerWithDropOff != null)
            {

             

                if (lockerWithDropOff.StoragePeriodEnd < DateTime.Now)
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.CodeExpired).MappedResponseValidityModel();
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.ContactNo, globalConfigurations.ContactNo);
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.AppName, globalConfigurations.AppName);
                    return model;
                }
               
            }

            var locker = await _lockerRepository.GetOTP(OTPCode);

            if (locker != null)
            {
                if (bookingStatus == (int)BookingTransactionStatus.ForPickUp)
                {

            

                        if (locker.StoragePeriodEnd < DateTime.Now)
                        {
                            model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.CodeExpired).MappedResponseValidityModel();
                            model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.ContactNo, globalConfigurations.ContactNo);
                            model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.AppName, globalConfigurations.AppName);
                            return model;
                        }
                    
                }
            } 
          




            if (locker == null)
            {


                var lockerStatus = await _lockerRepository.GetOTPStatus(OTPCode);
                if (lockerStatus != null)
                {
                    if (bookingStatus == (int)BookingTransactionStatus.ForDropOff)
                    {
                        if (lockerStatus.DropOffCode == OTPCode)
                        {
                    

                            if (lockerStatus.StoragePeriodEnd < DateTime.Now || lockerStatus.BookingStatus == (int)BookingTransactionStatus.Expired)
                            {
                                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.CodeExpired).MappedResponseValidityModel();
                                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.ContactNo, globalConfigurations.ContactNo);
                                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.AppName, globalConfigurations.AppName);
                                return model;
                            }
                            
                        }
                        else
                        {
                            model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.IncorrectDropOffCode).MappedResponseValidityModel();
                            return model;
                        }
                    }
                    if (bookingStatus == (int)BookingTransactionStatus.ForPickUp)
                    {
                        if (!string.IsNullOrEmpty(lockerStatus.PickUpCode))
                        {
                            if (lockerStatus.PickUpCode == OTPCode)
                            {  

                                    if (locker.StoragePeriodEnd < DateTime.Now || lockerStatus.BookingStatus == (int)BookingTransactionStatus.Expired)
                                    {
                                        model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.CodeExpired).MappedResponseValidityModel();
                                        model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.ContactNo, globalConfigurations.ContactNo);
                                        model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.AppName, globalConfigurations.AppName);
                                        return model;
                                    }
                                
                               
                            }
                            else
                            {
                                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.IncorrectCollectCode).MappedResponseValidityModel();
                                return model;
                            }

                        }
                        else

                        {
                            model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.IncorrectCollectCode).MappedResponseValidityModel();
                            return model;
                        }
                    }
                }

                if (bookingStatus == (int)BookingTransactionStatus.ForDropOff)
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.IncorrectDropOffCode).MappedResponseValidityModel();
                else if (bookingStatus == (int)BookingTransactionStatus.ForPickUp)
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.IncorrectCollectCode).MappedResponseValidityModel();
                else
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.IncorrectOTP).MappedResponseValidityModel();
                return model;
            }
            var lockerDetails = await _lockerRepository.GetActiveLocker(lockerDetailId: locker.LockerDetailId, cabinetId: cabinetId);
            if (lockerDetails.Count < 1)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
                _logger.LogError("OTP Validation for OTP " + OTPCode + " incorrect due to no active locker from cabinetId " + cabinetId + " and lockerDetailID " + locker.LockerDetailId);
                return model;
            }
            if (companyId.HasValue && lockerDetails.First().CompanyId != companyId)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
                _logger.LogError("OTP Validation for OTP " + OTPCode + " incorrect due to null or non existend company Id from cabinetId " + cabinetId + " and lockerDetailID " + locker.LockerDetailId);

                return model;
            }
            var userSubscriptions = await userSubscriptionRepository.GetUserSubscription(locker.UserKeyId, locker.LockerDetailId, locker.StoragePeriodEnd);
            bool isSubscribed = userSubscriptions.Count > 0;
            if (DateTime.Now < locker.StoragePeriodStart
                && !isSubscribed
                && (bookingStatus == (int)BookingTransactionStatus.ForDropOff
                || bookingStatus == (int)BookingTransactionStatus.ForPickUp))
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.CodeNotLiveYet).MappedResponseValidityModel();
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.Type, bookingStatus == (int)BookingTransactionStatus.ForDropOff ? "Drop Off" : "Pick Up");
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.StartDate, locker.StoragePeriodStart.ToString("MM-dd-yyyy"));
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.StartTime, locker.StoragePeriodStart.ToString("hh:mm tt"));
                return model;
            }
            var userSubscription = userSubscriptions.FirstOrDefault();
          
 

            if ((locker.StoragePeriodEnd < DateTime.Now && !isSubscribed) ||
                (userSubscription != null && userSubscription.ExpiryDate < DateTime.Now && isSubscribed))
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.CodeExpired).MappedResponseValidityModel();
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.ContactNo, globalConfigurations.ContactNo);
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.AppName, globalConfigurations.AppName);
                return model;
            }
            /*
            if (isSubscribed && userSubscription != null && DateTime.Now > userSubscription.ExpiryDate && (locker.BookingStatus == 1 || locker.BookingStatus == 2))
            {
                var existingCharges = await _userService.GetChargesByUser(locker.UserKeyId, locker.PaymentReference);
                //(charges already created or not)
                if (existingCharges.Count < 1)
                {
                    var result = await ProcessOverStayCharges(locker, expiryDate: userSubscription.ExpiryDate);
                    if (result != null) return result;
                }
                else if (existingCharges.Count > 0 && existingCharges.Any(s => s.PaymentStatus != "paid"))
                {
                    var result = await ProcessOverStayCharges(locker, existingCharges.First(), expiryDate: userSubscription.ExpiryDate);
                    if (result != null) return result;
                }
                //Validation(Charges already paid or not)
                else if (existingCharges.Any(s => s.PaymentStatus == "paid"))
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.ValidOTP).MappedResponseValidityModel();
                    model.LockerNumber = locker.LockerNumber;
                    model.LockerTransactionsId = locker.LockerTransactionsId;
                    model.BoardNumber = locker.BoardNumber;
                    model.GetStatusCommand = locker.GetStatusCommand;
                    model.OpenCommand = locker.OpenCommand;
                }
            }*/
            /*
            if (!isSubscribed && DateTime.Now > locker.StoragePeriodEnd && (locker.BookingStatus == 1 || locker.BookingStatus == 2))
            {
                var existingCharges = await _userService.GetChargesByUser(locker.UserKeyId, locker.PaymentReference);
                //(charges already created or not)
                if (existingCharges.Count < 1)
                {
                    var result = await ProcessOverStayCharges(locker);
                    if (result != null) return result;
                }
                else if (existingCharges.Count > 0 && existingCharges.Any(s => s.PaymentStatus != "paid"))
                {
                    var result = await ProcessOverStayCharges(locker, existingCharges.First());
                    if (result != null) return result;
                }
                //Validation(Charges already paid or not)
                else if (existingCharges.Any(s => s.PaymentStatus == "paid"))
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.ValidOTP).MappedResponseValidityModel();
                    model.LockerNumber = locker.LockerNumber;
                    model.LockerTransactionsId = locker.LockerTransactionsId;
                    model.BoardNumber = locker.BoardNumber;
                    model.GetStatusCommand = locker.GetStatusCommand;
                    model.OpenCommand = locker.OpenCommand;
                }
            }*/

            if (isSubscribed && locker.DropOffCode == OTPCode && bookingStatus == GlobalEnums.BookingTransactionStatus.ForDropOff.GetHashCode())
            {
                if (!DateTime.Now.DateTimeInRange(locker.StoragePeriodStart, userSubscription.ExpiryDate))
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnlockNotYetDue).MappedResponseValidityModel();
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.Type, MessageReplacement.DropOff);
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.StartDate, locker.StoragePeriodStart.ToShortDateString());
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.EndDate, userSubscription.ExpiryDate.ToShortDateString());

                    return model;
                }

                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.DoorOpen).MappedResponseValidityModel();
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.LockerNumber, locker.LockerNumber);
                model.LockerNumber = locker.LockerNumber;
                model.LockerTransactionsId = locker.LockerTransactionsId;
                model.BoardNumber = locker.BoardNumber;
                model.GetStatusCommand = locker.GetStatusCommand;
                model.OpenCommand = locker.OpenCommand;
                return model;
            }
            if (!isSubscribed && locker.DropOffCode == OTPCode && bookingStatus == GlobalEnums.BookingTransactionStatus.ForDropOff.GetHashCode())
            {
                if (!DateTime.Now.DateTimeInRange(locker.StoragePeriodStart, locker.StoragePeriodEnd))
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnlockNotYetDue).MappedResponseValidityModel();
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.Type, MessageReplacement.DropOff);
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.StartDate, locker.StoragePeriodStart.ToString());
                    // model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.EndDate, locker.StoragePeriodEnd.ToShortDateString());

                    return model;
                }
                model.LockerNumber = locker.LockerNumber;
                model.LockerTransactionsId = locker.LockerTransactionsId;
                model.BoardNumber = locker.BoardNumber;
                model.GetStatusCommand = locker.GetStatusCommand;
                model.OpenCommand = locker.OpenCommand;
             //   await SendBookingNotification(locker.LockerTransactionsId, NotificationType.ForDropOffSuccessNotification);
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.NotificationSent).MappedResponseValidityModel();
                return model;
            }
            if (locker.PickUpCode == OTPCode && bookingStatus == GlobalEnums.BookingTransactionStatus.ForPickUp.GetHashCode())
            {
                model.LockerNumber = locker.LockerNumber;
                model.LockerTransactionsId = locker.LockerTransactionsId;
                model.BoardNumber = locker.BoardNumber;
                model.GetStatusCommand = locker.GetStatusCommand;
                model.OpenCommand = locker.OpenCommand;
              //await SendBookingNotification(locker.LockerTransactionsId, NotificationType.ForCollectSuccessNotification);
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.NotificationSent).MappedResponseValidityModel();
            }
            else
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.IncorrectOTP).MappedResponseValidityModel();
            }
            return model;
        }


        private async Task<OTPModel> ProcessOverStayCharges(LockerBookingEntity locker, ChargesViewModel existingCharges = null, DateTime? expiryDate = null)
        {
            var model = new OTPModel();
            var lockerDetail = await _lockerRepository.GetLockerDetail(locker.LockerDetailId);
            if (lockerDetail.Count > 0)
            {
                if (!expiryDate.HasValue)
                    expiryDate = locker.StoragePeriodEnd;
                var selectedLocker = lockerDetail.First();
                var charges = await priceAndChargingRepository.Get(selectedLocker.LockerTypeId, selectedLocker.CabinetLocationId);
                decimal calculatedCharges = 0;
                if (charges.Count > 0)
                {
                    var currentOverStayCharge = charges.First();
                    var totalDays = (int)DateTime.Now.Subtract(expiryDate.Value).TotalDays;
                    var totalHours = (int)DateTime.Now.Subtract(expiryDate.Value).TotalHours;
                    var totalMonths = (int)(DateTime.Now.Subtract(expiryDate.Value).TotalDays / 30);

                    if ((currentOverStayCharge.PricingType.ToLower() == "daily" && totalDays > currentOverStayCharge.OverstayPeriod) ||
                        (currentOverStayCharge.PricingType.ToLower() == "monthly" && totalMonths > currentOverStayCharge.OverstayPeriod) ||
                        (currentOverStayCharge.PricingType.ToLower() == "hourly" && totalHours > currentOverStayCharge.OverstayPeriod))
                    {
                        calculatedCharges = CalculateOverStayCharge(locker.TotalPrice, currentOverStayCharge.OverstayCharge, currentOverStayCharge.PricingType, DateTime.Now, locker.StoragePeriodEnd);
                        if (calculatedCharges > 0)
                            model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.OverStayCharges).MappedResponseValidityModel();
                    }
                    //In case totalDays < DefaultOverstayPeriod
                    else
                    {
                        calculatedCharges = CalculateOverStayCharge(locker.TotalPrice, currentOverStayCharge.OverstayCharge, currentOverStayCharge.PricingType, DateTime.Now, locker.StoragePeriodEnd);
                        if (calculatedCharges > 0)
                        {
                            var message = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.IncuredCharges);
                            model.ValidityModel.Message = message.Message.Replace("{Charges amount}", calculatedCharges.ToString());
                        }

                    }
                    if (calculatedCharges > 0)
                    {
                        if (existingCharges == null)
                            await _userService.SaveCharges(new ChargesModel
                            {
                                BookingAmount = locker.TotalPrice,
                                BookingDate = locker.DateCreated,
                                Charges = calculatedCharges,
                                LockerDetailId = locker.LockerDetailId,
                                LockerTypeId = selectedLocker.LockerTypeId,
                                TotalAmount = locker.TotalPrice,
                                UserKeyId = locker.UserKeyId

                            });
                        else
                            await _userService.SaveCharges(new ChargesModel
                            {
                                Id = existingCharges.Id,
                                BookingAmount = locker.TotalPrice,
                                BookingDate = locker.DateCreated,
                                Charges = calculatedCharges,
                                LockerDetailId = locker.LockerDetailId,
                                LockerTypeId = selectedLocker.LockerTypeId,
                                TotalAmount = locker.TotalPrice,
                                UserKeyId = locker.UserKeyId

                            });

                        return model;
                    }
                }
            }
            return null;
        }
        public async Task<OTPModel> UpdateOTP(string OTPCode)
        {
            var locker = await _lockerRepository.GetOTP(OTPCode);
            var pin = await _iOTPService.GenerateOTP();
            var qrCode = string.Concat(locker.LockerDetailId, pin);
            var user = await _userService.GetUserByUserKeyId(locker.UserKeyId);

            var model = new OTPModel();

            if (locker.BookingStatus == 1 && locker.DropOffCode == OTPCode)
            {
                var ret = await _lockerRepository.UpdatedOTP(BookingTransactionStatus.ForDropOff, pin, qrCode, locker.LockerTransactionsId);
                if (ret > 0)
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.ValidOTP).MappedResponseValidityModel();
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(GlobalConstants.MessageParameters.Pin, pin);
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(GlobalConstants.MessageParameters.Type, "Pick-up");
                    model.OTP = pin;
                    model.QRCode = qrCode;
                    return model;
                }
                else
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NoItemSave).MappedResponseValidityModel();
                    return model;
                }
            }

            return model;
        }

        public async Task<LockerBookingEntity> GetLockerBooking(int lockertransactionId)
        {
            return await _lockerRepository.GetLockerBooking(lockertransactionId);
        }

        public async Task<ResponseValidityModel> SetLockerDetailActivation(int id, bool isDeleted)
        {
            var ret = await _lockerRepository.SetLockerDetailActivation(id, isDeleted);

            if (ret > 0)
            {
                return AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
            {
                return AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            }
        }

        public async Task<List<LockerBookingHistoryModel>> GetDropOffHistory(string userKeyId)
        {
            var dbModel = await _lockerRepository.GetDropOffHistory(userKeyId);
            if (dbModel == null)
                return new List<LockerBookingHistoryModel>();

            return dbModel;

        }
        public async Task<List<LockerBookingHistoryModel>> GetPickupHistory(string userKeyId)
        {

            var dbModel = await _lockerRepository.GetPickupHistory(userKeyId);
            if (dbModel == null)
                return new List<LockerBookingHistoryModel>();

            return dbModel;
        }

        async Task<ResponseValidityModel> ValidateLockerZone(LockerZoneModel lockerZone)
        {
            var model = new ResponseValidityModel();

            var type = await GetLockerZones(lockerZone.Name, lockerZone.PositionId);

            if (type.Any())
            {
                model = AppMessageService.SetMessage(GlobalConstants.ApplicationMessageNumber.ErrorMessage.LockerZoneAlreadyExists).MappedResponseValidityModel();
            }
            return model;
        }
        public async Task<ResponseValidityModel> SaveLockerZone(LockerZoneModel lockerZone)
        {
            var model = await ValidateLockerZone(lockerZone);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<LockerZoneModel, LockerZoneEntity>(lockerZone);
                var ret = await _lockerRepository.SaveLockerZone(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }

            return model;
        }

        public async Task<ResponseValidityModel> DeleteLockerZone(int id)
        {

            var model = new ResponseValidityModel();

            if (id > 0)
            {

                var ret = await _lockerRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;



            return model;
        }

        public async Task<List<LockerZoneModel>> GetLockerZones(string name = null, int? currentId = null)
        {
            var dbModel = await _lockerRepository.GetLockerZones(name, currentId);
            var model = new List<LockerZoneModel>();
            model = Mapper.Map<List<LockerZoneModel>>(dbModel);
            return model;
        }
        public async Task<List<ReassignedBookingLockerModel>> GetReassignedBookingLockerHistoryForAdmin(int? lockerDetailId = null, int? lockerTransactionsId = null, int? adminUserId = null, int? companyUserId = null, int? companyId = null)
        {
            var dbModel = await _lockerRepository.GetReassignedBookingLockerHistory(lockerDetailId, lockerTransactionsId, adminUserId, companyUserId, companyId);
            return Mapper.Map<List<ReassignedBookingLockerModel>>(dbModel);
        }
        public async Task<ResponseValidityModel> ReassignBookingForAdmin(BookingLockerDetailModel param, string userKeyId)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(param.lockerNumber)
                || param.CabinetId < 1
                || param.lockerTransactionId < 1) model.MessageReturnNumber = 1;
            if (model.MessageReturnNumber == 0)
            {
                var adminUser = await _userService.GetAdminUser(userKeyId, false);
                if (adminUser == null)
                {
                    model.MessageReturnNumber = 1;
                    return model;
                }
                var lst = await _lockerRepository.GetLockerDetail(lockerNumber: param.lockerNumber, cabinetId: param.CabinetId);
                if (lst.Count > 0)
                {
                    var newLockerDetailId = lst.First().LockerDetailId;
                    var existingBooking = await _lockerRepository.GetLockerBooking(param.lockerTransactionId);
                    if (existingBooking == null || existingBooking.LockerDetailId == newLockerDetailId)
                    {
                        model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                        model.Message = model.Message.Replace(MessageParameters.Field, "LockerTransactionsId");
                        return model;
                    }
                    else
                    {


                        var newOtpCode = await _iOTPService.GenerateOTP();
                        var newOtpQRCode = string.Concat(newLockerDetailId, newOtpCode);
                        var ret = await _lockerRepository.ReassignBooking(existingBooking, param, newLockerDetailId, newOtpCode, newOtpQRCode, adminUserId: adminUser.AdminUserId);
                        var ret2 = await _lockerRepository.SetLockerDetailActivation(existingBooking.LockerDetailId, false);


                        await SendBookingNotification(param.lockerTransactionId, NotificationType.ForLockerReassignmentNotification);
                        

                        model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
                    }
                }
                else
                {
                    model.MessageReturnNumber = 1;
                    return model;
                }
            }
            return model;
        }
        public async Task<List<ReassignedBookingLockerViewModel>> GetReassignedBookingLockerHistoryForCompanyUser(string userKeyId, int? lockerDetailId = null, int? lockerTransactionsId = null, int? companyUserId = null)
        {
            var companyUser = await companyUserRepository.GetCompanyUser(username: userKeyId);
            if (companyUser.Count < 1) return new List<ReassignedBookingLockerViewModel>();
            var currentUser = companyUser.First();
            var dbModel = await _lockerRepository.GetReassignedBookingLockerHistory(lockerDetailId, lockerTransactionsId, null, companyUserId, companyId: currentUser.CompanyId);
            return Mapper.Map<List<ReassignedBookingLockerViewModel>>(dbModel);
        }
        public async Task<ResponseValidityModel> ReassignBookingForCompanyUser(BookingLockerDetailModel param, string userKeyId)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(param.lockerNumber)
                || param.CabinetId < 1
                || param.lockerTransactionId < 1) model.MessageReturnNumber = 1;

            if (model.MessageReturnNumber == 0)
            {
                var companyUsers = await companyUserRepository.GetCompanyUser(username: userKeyId);
                if (companyUsers.Count < 1)
                {
                    model.MessageReturnNumber = 1;
                    return model;
                }
                var currentUser = companyUsers.First();
                var isValid = await _lockerRepository.IsValidTransaction(param.lockerTransactionId, currentUser.CompanyId);
                if (!isValid)
                {
                    model.MessageReturnNumber = 1;
                    return model;
                }
                var lst = await _lockerRepository.GetLockerDetail(lockerNumber: param.lockerNumber, cabinetId: param.CabinetId);
                if (lst.Count > 0)
                {
                    var existingBooking = await _lockerRepository.GetLockerBooking(param.lockerTransactionId);
                    if (existingBooking == null)
                    {
                        model.MessageReturnNumber = 1;
                        return model;
                    }
                    else
                    {
                        var newOtpCode = await _iOTPService.GenerateOTP();
                        var newOtpQRCode = string.Concat(lst.First().LockerDetailId, newOtpCode);
                        var ret = await _lockerRepository.ReassignBooking(existingBooking, param, lst.First().LockerDetailId, newOtpCode, newOtpQRCode, companyUserId: currentUser.CompanyUserId);
                        model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
                    }
                }
                else
                {
                    model.MessageReturnNumber = 1;
                    return model;
                }
            }
            return model;
        }
        public async Task<bool> SendBookingReceiptEmail(BookingReceiptModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name)
                || string.IsNullOrEmpty(model.Currency)
                || string.IsNullOrEmpty(model.ContactNo)
                || string.IsNullOrEmpty(model.UserEmail)
                || model.Detail.Count < 1
                || model.TotalPrice != model.Detail.Sum(s => s.TotalPrice)
                || model.TotalPrice < 0 || !SharedServices.IsValidEmail(model.UserEmail)) return false;
            return await _notificationService.SendBookingReceiptEmail(model);
        }
        public async Task<UserLockerBookingReportModel> GetBookingTransactionsReport(string userKeyId)
        {
            if (string.IsNullOrEmpty(userKeyId))
                return new UserLockerBookingReportModel();
            return await _lockerRepository.GetBookingTransactionsReport(userKeyId);
        }



        public async Task<UserLockerBookingReportModel> GetBookingUserTransactionsReport(string userKeyId)
        {
            if (string.IsNullOrEmpty(userKeyId))
                return new UserLockerBookingReportModel();
            return await _lockerRepository.GetBookingTransactionsReport(userKeyId);
        }


        public async Task<int> GetActiveBookingCount(
            int lockerDetailId, DateTime storagePeriodStartDate,
            DateTime storagePeriodEndDate, int? lockerTransactionsId = null)
        {
            return await _lockerRepository.ActiveBookingsCount(lockerDetailId, storagePeriodStartDate, storagePeriodEndDate, lockerTransactionsId);
        }
        public async Task<ResponseValidityModel> CancelBooking(CancelBookingModel model, string UserKeyId)
        {
            var response = ValidateCancelBooking(model);
            if (response.MessageReturnNumber > 0)
                return AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidInput).MappedResponseValidityModel();
            var bookingDetail = await _lockerRepository.GetLockerBooking(model.lockerTransactionsId);
            if (bookingDetail == null || bookingDetail.StoragePeriodStart <= DateTime.Now
                || (bookingDetail.BookingStatus != 1 && bookingDetail.BookingStatus != 2)
                || bookingDetail.UserKeyId != UserKeyId)
                return AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.TransactionNotExisting).MappedResponseValidityModel();
            var payment = await paymentService.GetPaymentTransaction(model.paymentReferenceId);
            if (payment == null)
                return AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.PaymentNotFound).MappedResponseValidityModel();
            if (payment.Status != PayamayaStatus.PaymentSuccess)
                return AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.UnpaidPayment).MappedResponseValidityModel();
            var ret = await _lockerRepository.CancelLockerBooking(model, bookingDetail, UserKeyId);
            if (ret > 0)
                response = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.RecordUpdated).MappedResponseValidityModel();
            else
                response = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            return response;
        }
        ResponseValidityModel ValidateCancelBooking(CancelBookingModel model)
        {
            var response = new ResponseValidityModel();
            if (model.lockerTransactionsId < 1 || string.IsNullOrEmpty(model.paymentReferenceId))
                response.MessageReturnNumber = 1;

            return response;
        }

        public async Task<LockerBookingViewModel> GetLockerBookingByTransactionId(int lockerTransactionId)
        {
            var dbModel = await _lockerRepository.GetLockerBooking(lockerTransactionId);
            var model = new LockerBookingViewModel();

            if (dbModel != null)
                model = Mapper.Map<LockerBookingViewModel>(dbModel);

            return model;

        }
        public async Task<List<LockerBookingViewModel>> GetUserBookings(string userkeyId,
            BookingTransactionStatus? bookingStatus = null, DateTime? fromDate = null,
            DateTime? toDate = null, int? currentPage = null, int? pageSize = null, bool activeOnly = false)
        {
            var model = new List<LockerBookingViewModel>();
            if (string.IsNullOrEmpty(userkeyId)) return model;
            var dbModel = await _lockerRepository.GetUserBookings(userkeyId, bookingStatus, fromDate, toDate, currentPage, pageSize, activeOnly);


            if (dbModel != null && dbModel.Count > 0)
            {
                var groupBy = dbModel.GroupBy(s => s.LockerTransactionsId).Select(s => new { key = s.Key, lst = s.ToList() });
                foreach (var item in groupBy)
                {
                    var first = item.lst.First();
                    var bookingDetail = Mapper.Map<LockerBookingViewModel>(first);
                    bookingDetail.Payments = Mapper.Map<List<PaymentTransactionModel>>(item.lst);
                    model.Add(bookingDetail);
                }
                model.OrderByDescending(s => s.StoragePeriodStart);
            }


            return model;
        }

        public async Task<ResponseValidityModel> UpdateLockerBookingStatus(PostUpdateBookingStatusModel postUpdateBookingStatusModel)
        {
            ResponseValidityModel response;
            var dbModel = await _lockerRepository.GetLockerBookingByTransactionId(postUpdateBookingStatusModel.LockerTransactionId);

            if (Enum.IsDefined(typeof(GlobalEnums.BookingTransactionStatus), postUpdateBookingStatusModel.BookingStatus))
            {
                if (dbModel == null)
                {
                    response = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.NoRecords).MappedResponseValidityModel();
                }
                else if (dbModel.BookingStatus != BookingTransactionStatus.ForDropOff.GetHashCode() && dbModel.BookingStatus != BookingTransactionStatus.ForPickUp.GetHashCode()
                    && dbModel.BookingStatus != BookingTransactionStatus.Confiscated.GetHashCode())
                {
                    response = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.BookingTransactionAlreadyComplete).MappedResponseValidityModel();
                }
                else if (postUpdateBookingStatusModel.BookingStatus != BookingTransactionStatus.Confiscated)
                {
                    if (dbModel.BookingStatus != BookingTransactionStatus.Confiscated.GetHashCode())
                             response = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.BookingStatusUpdate).MappedResponseValidityModel();
                    else
                    {
                        var ret = await _lockerRepository.UpdateBookingStatus(postUpdateBookingStatusModel.LockerTransactionId, postUpdateBookingStatusModel.BookingStatus.GetHashCode());
                        response = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
                    }


                }
                else
                {
                    var ret = await _lockerRepository.UpdateBookingStatus(postUpdateBookingStatusModel.LockerTransactionId, postUpdateBookingStatusModel.BookingStatus.GetHashCode());
                    response = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
                }
            }
            else
            {
                response = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                response.Message = response.Message.Replace(GlobalConstants.MessageParameters.Field, MessageReplacement.BookingTransaction);
            }

            return response;
        }
        public async Task SendMultiBookingDropOffEmail(List<int> lockerTransactionids)
        {
            if (lockerTransactionids.Count < 1) return;
            var emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.MultiBookingDropOffCode).MappedResponseValidityModel().Message;
            var bookingDetail = "";
            var userName = "";
            var userEmail = "";
            foreach (var lockerTransactionid in lockerTransactionids)
            {
                var lockerTransactionDetail = await _lockerRepository.GetLockerBookingDetail(lockerTransactionid);
                if (lockerTransactionDetail != null)
                {
                    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
                    {
                        userName = lockerTransactionDetail.UserFirstName;
                        userEmail = lockerTransactionDetail.UserEmail;
                    }
                    bookingDetail += $"Address: {lockerTransactionDetail.CabinetLocationDescription}<br/>";
                    bookingDetail += $"Locker Number: {lockerTransactionDetail.LockerNumber}<br/>";
                    bookingDetail += $"Storage From: {lockerTransactionDetail.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper()}<br/>";
                    bookingDetail += $"Storage To: {lockerTransactionDetail.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper()}<br/>";
                    bookingDetail += $"Drop Off PIN Code: {lockerTransactionDetail.DropOffCode}<br/>";
                    bookingDetail += $"Reference Code: {lockerTransactionDetail.PaymentReference}<br/>";
                    bookingDetail += "<br/><br/>";
                }
            }

            emailMsg = emailMsg.Replace(MessageParameters.UserName, userName);
            emailMsg = emailMsg.Replace(MessageParameters.OtherInfo, "Your booking details are as follows:");
            emailMsg = emailMsg.Replace(MessageParameters.BookingDetail, bookingDetail);

            if (!string.IsNullOrEmpty(emailMsg) && !string.IsNullOrEmpty(userEmail))
            {
                var isSent = await _notificationService.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = "Your POBOxX booking is confirmed",
                    Message = emailMsg,
                    To = userEmail,
                    Type = MessageType.HtmlContent
                });

                if (isSent)
                    _logger.LogInformation($"Success sending of email for Multi Booking DropOfF notification to user with email " + userEmail);
                else
                    _logger.LogError($"Failed sending of email for Multi Booking DropOfF notification to user with email " + userEmail);

            }
        }

        public async Task SendBookingCancellationNotification(LockerMobileBookingModel lockerBookingEntity)
        {
            
          
           
            var smsMsg = "";
            var emailMsg = "";
            var subject = "";
 
            var refCode = lockerBookingEntity.PaymentReference;
            var lockerDetails = await _lockerRepository.GetLockerDetail(lockerBookingEntity.LockerDetailId);
            if (lockerDetails.Count < 1) return;
            var currentLocker = lockerDetails.First();
            //Message For SMS
            smsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.LockerUnavailableAfterPayment).MappedResponseValidityModel().Message;
                smsMsg = smsMsg.Replace(MessageParameters.StartDate, lockerBookingEntity.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.EndDate, lockerBookingEntity.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
              
                smsMsg = smsMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                smsMsg = smsMsg.Replace(MessageParameters.LockerNumber, currentLocker.LockerNumber);
            smsMsg = smsMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);

            emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.LockerUnavailableAfterPaymentLongMsg).MappedResponseValidityModel().Message;
                emailMsg = emailMsg.Replace(MessageParameters.UserName, lockerBookingEntity.ReceiverName);
                emailMsg = emailMsg.Replace(MessageParameters.StartDate, lockerBookingEntity.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper()); 
                emailMsg = emailMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
            emailMsg = emailMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
            emailMsg = emailMsg.Replace(MessageParameters.LockerNumber, currentLocker.LockerNumber);
                emailMsg = emailMsg.Replace(MessageParameters.EndDate, lockerBookingEntity.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                emailMsg = emailMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());


            //SMS
            if (!string.IsNullOrEmpty(smsMsg))
            {



                var isSent =  _httpService.SendSMS(lockerBookingEntity.ReceiverPhoneNumber, smsMsg, refCode).Result;

           

                if (isSent)
                    _logger.LogInformation($"Success sending of SMS for payment cancellation having {refCode} to user with Phone Number " + lockerBookingEntity.ReceiverPhoneNumber);
                else
                    _logger.LogError($"Failed sending of SMS for payment cancellation having {refCode} to user with Phone Number " + lockerBookingEntity.ReceiverPhoneNumber);
            }
            //EMAIL
            if (!string.IsNullOrEmpty(emailMsg))
            {

                var isSent = _notificationService.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = subject,
                    Message = emailMsg,
                    To = lockerBookingEntity.ReceiverEmailAddress,
                    Type = MessageType.HtmlContent
                }).Result;

                if (isSent)
                    _logger.LogInformation($"Success sending of email for payment cancellationhaving {refCode} to user with email " + lockerBookingEntity.ReceiverEmailAddress);
                else
                    _logger.LogError($"Failed sending of email for payment cancellationv having {refCode} to user with email " + lockerBookingEntity.ReceiverEmailAddress);

            }

        }
        public async Task SendBookingNotification(int lockerTransactionId, NotificationType notificationType)
        {
            var lockerBooking = await _lockerRepository.GetLockerBooking(lockerTransactionId);
            if (lockerBooking == null) return;
            var userDetail = await _userService.GetUserByUserKeyId(lockerBooking.UserKeyId);
            if (userDetail == null) return;
            var smsMsg = "";
            var emailMsg = "";
            var subject = "";
            var lockerDetails = await _lockerRepository.GetLockerDetail(lockerBooking.LockerDetailId);
            if (lockerDetails.Count < 1) return;
            var currentLocker = lockerDetails.First();
            var refCode = lockerBooking.PaymentReference;

            if (notificationType == NotificationType.ForDropOffPin)
            {
                subject = "Booking Confirmed";
                //Message For SMS
                smsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingDropOffPinShortMessage).MappedResponseValidityModel().Message;
                smsMsg = smsMsg.Replace(MessageParameters.StartDate, lockerBooking.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.EndDate, lockerBooking.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.PinCode, lockerBooking.DropOffCode);
                smsMsg = smsMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.LockerAddress, lockerBooking.CabinetLocationDescription);
                smsMsg = smsMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                smsMsg = smsMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                ////Message For Email
                //emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingDropOffPinLongMessage).MappedResponseValidityModel().Message;
                //emailMsg = emailMsg.Replace(MessageParameters.UserName, userDetail.FirstName);
                //emailMsg = emailMsg.Replace(MessageParameters.PlanType, lockerBooking.IsSubscriptionBooking ? "multi access" : "single access");
                //emailMsg = emailMsg.Replace(MessageParameters.StartDate, lockerBooking.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                //emailMsg = emailMsg.Replace(MessageParameters.EndDate, lockerBooking.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                //emailMsg = emailMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                //emailMsg = emailMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                //emailMsg = emailMsg.Replace(MessageParameters.PinCode, lockerBooking.DropOffCode);
                //emailMsg = emailMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                //emailMsg = emailMsg.Replace(MessageParameters.Refcode, refCode);
            }
            if (notificationType == NotificationType.ForDropOffSuccessNotification)
            {
                subject = "Parcel has been dropped Off";
                //Message For SMS
                smsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingDropOffSuccessShortMessage).MappedResponseValidityModel().Message;
                smsMsg = smsMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                smsMsg = smsMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                smsMsg = smsMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                //Message For Email
                emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingDropOffSuccessLongMessage).MappedResponseValidityModel().Message;
                emailMsg = emailMsg.Replace(MessageParameters.UserName, userDetail.FirstName);
                emailMsg = emailMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                emailMsg = emailMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                emailMsg = emailMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                emailMsg = emailMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
            }
            if (notificationType == NotificationType.ForCollectPin)
            {
                subject = "Collect Pin Notification";
                //Message For SMS
                smsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingCollectPinShortMessage).MappedResponseValidityModel().Message;
                smsMsg = smsMsg.Replace(MessageParameters.PinCode, lockerBooking.PickUpCode);
                smsMsg = smsMsg.Replace(MessageParameters.EndDate, lockerBooking.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                smsMsg = smsMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                smsMsg = smsMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                smsMsg = smsMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.Name, userDetail.FirstName);
                //Message For Email
                emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingCollectPinLongMessage).MappedResponseValidityModel().Message;
                emailMsg = emailMsg.Replace(MessageParameters.UserName, userDetail.FirstName);
                emailMsg = emailMsg.Replace(MessageParameters.PinCode, lockerBooking.PickUpCode);
                emailMsg = emailMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                emailMsg = emailMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                emailMsg = emailMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                emailMsg = emailMsg.Replace(MessageParameters.EndDate, lockerBooking.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                emailMsg = emailMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                emailMsg = emailMsg.Replace(MessageParameters.Name, userDetail.FirstName);

            }

            if (notificationType == NotificationType.ForCollectSuccessNotification)
            {
                subject = "Parcel has been collected";
                //Message For SMS
                smsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingCollectSuccessMessage).MappedResponseValidityModel().Message;
                smsMsg = smsMsg.Replace(MessageParameters.Refcode, refCode.ToUpper()) ;
                smsMsg = smsMsg.Replace(MessageParameters.ReceiverName, lockerBooking.ReceiverName);
                smsMsg = smsMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                smsMsg = smsMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                smsMsg = smsMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                //Message For Email
                emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.BookingCollectSuccessLongMessage).MappedResponseValidityModel().Message;
                emailMsg = emailMsg.Replace(MessageParameters.UserName, userDetail.FirstName);
                emailMsg = emailMsg.Replace(MessageParameters.ReceiverName, lockerBooking.ReceiverName);
                emailMsg = emailMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                emailMsg = emailMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                emailMsg = emailMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                emailMsg = emailMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
            }
            if (notificationType == NotificationType.ForBookingExtentionSuccessNotification)
            {
                subject = "Your Booking has been extended";
                //Message For SMS
                smsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerBookingExtendedShortMessage).MappedResponseValidityModel().Message;
                smsMsg = smsMsg.Replace(MessageParameters.StartDate, lockerBooking.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.EndDate, lockerBooking.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.PlanType, lockerBooking.AccessPlan == 2 ? "multi access" : "single access");
                smsMsg = smsMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                smsMsg = smsMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                smsMsg = smsMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);

                //Message For Email
                emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerBookingExtendedLongMessage).MappedResponseValidityModel().Message;
                emailMsg = emailMsg.Replace(MessageParameters.UserName, userDetail.FirstName);
                emailMsg = emailMsg.Replace(MessageParameters.PlanType, lockerBooking.AccessPlan == 2 ? "multi access" : "single access");
                emailMsg = emailMsg.Replace(MessageParameters.StartDate, lockerBooking.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                emailMsg = emailMsg.Replace(MessageParameters.EndDate, lockerBooking.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                emailMsg = emailMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                emailMsg = emailMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                emailMsg = emailMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                emailMsg = emailMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
            }

            if (notificationType == NotificationType.ForLockerReassignmentNotification)
            {
                subject = "Locker Reassinged";
                //Message For SMS
                smsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerReassignedShort).MappedResponseValidityModel().Message;
                smsMsg = smsMsg.Replace(MessageParameters.StartDate, lockerBooking.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.EndDate, lockerBooking.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.PinCode, lockerBooking.DropOffCode);
                smsMsg = smsMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                smsMsg = smsMsg.Replace(MessageParameters.PlanType, lockerBooking.AccessPlan == 2 ? "multi access" : "single access");
                smsMsg = smsMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                smsMsg = smsMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                smsMsg = smsMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                //Message For Email
                emailMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerReassignedLong).MappedResponseValidityModel().Message;
                emailMsg = emailMsg.Replace(MessageParameters.UserName, userDetail.FirstName);
                emailMsg = emailMsg.Replace(MessageParameters.PlanType, lockerBooking.AccessPlan==2? "multi access" : "single access");
                emailMsg = emailMsg.Replace(MessageParameters.StartDate, lockerBooking.StoragePeriodStart.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                emailMsg = emailMsg.Replace(MessageParameters.EndDate, lockerBooking.StoragePeriodEnd.ToString("MM/dd/yyyy hh:mm tt").ToUpper());
                emailMsg = emailMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                emailMsg = emailMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                emailMsg = emailMsg.Replace(MessageParameters.PinCode, lockerBooking.DropOffCode);
                emailMsg = emailMsg.Replace(MessageParameters.LockerNumber, lockerBooking.LockerNumber);
                emailMsg = emailMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
            }
            //SMS
            if (!string.IsNullOrEmpty(smsMsg))
            {
               
           

               var isSent = await _httpService.SendSMS(userDetail.PhoneNumber, smsMsg, refCode);

                if (notificationType == NotificationType.ForCollectPin)
                { 
                    if (lockerBooking.ReceiverPhoneNumber.Substring(1) != userDetail.PhoneNumber)
                         isSent = await _httpService.SendSMS(lockerBooking.ReceiverPhoneNumber, smsMsg, refCode);
                }
                if (isSent)
                    _logger.LogInformation($"Success sending of SMS for {notificationType.ToString()} having {refCode} to user with Phone Number " + userDetail.PhoneNumber);
                else
                    _logger.LogError($"Failed sending of SMS for {notificationType.ToString()} having {refCode} to user with Phone Number " + userDetail.PhoneNumber);
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
                    _logger.LogInformation($"Success sending of email for {notificationType.ToString()} having {refCode} to user with email " + userDetail.Email);
                else
                    _logger.LogError($"Failed sending of email for {notificationType.ToString()} having {refCode} to user with email " + userDetail.Email);

            }
        }


       public async Task<bool> ValidateAvailableLocker(int cabinetLocationId, int lockerTypeId, int positionId,
           int lockerDetailId)
        {
            var model = new ResponseValidityModel();

                 var ret=  await _lockerRepository.ValidateAvailableLocker(cabinetLocationId, lockerTypeId, positionId
               ,lockerDetailId);

            if (ret > 0)
            {
                return false;           
            }else
                return true;



        }
     
      public async  Task<ResponseValidityModel> SelectLockerForBooking(int cabinetLocationId, int lockerTypeId, int positionId,
            DateTime storateStartDate, DateTime storageEndDate, string userKeyId, int lcokerDetailId)
        {
            var ret = await _lockerRepository.SelectLockerForBooking(cabinetLocationId, lockerTypeId, positionId,
                storateStartDate, storageEndDate, userKeyId, lcokerDetailId);

              var model = new ResponseValidityModel();


                ScheduleClearLockers(cabinetLocationId, lockerTypeId, positionId,
                                    userKeyId, lcokerDetailId, DateTime.Now);


                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            return model;
           
        }

       public async Task<ResponseValidityModel> ClearLockerForBooking(int cabinetLocationId, int lockerTypeId, int positionId,
             string userKeyId, int lcokerDetailId)
        {
        var ret = await _lockerRepository.ClearLockerForBooking(cabinetLocationId, lockerTypeId, positionId,
     userKeyId, lcokerDetailId);

        var model = new ResponseValidityModel();


        model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            _logger.LogInformation($"Locker deleted");

            return model;
    }

        public void ScheduleClearLockers(int cabinetLocationId, int lockerTypeId, int positionId,
            string userKeyId, int lockerDetailId, DateTime dateSelected)
        {

            _logger.LogInformation($"Schedule clear locker for lockerTransactionId {cabinetLocationId} and StorageEnd {dateSelected}");
            dateSelected = dateSelected.ToUniversalTime();
            //Scheduled job to clear selected locker
            JobScheduler.Schedule<ILockerService>(
        (s) => s.ClearLockerForBooking(cabinetLocationId, lockerTypeId,
        positionId, userKeyId,
        lockerDetailId),
        dateSelected.AddMinutes(5));

 
        }

    }
}
