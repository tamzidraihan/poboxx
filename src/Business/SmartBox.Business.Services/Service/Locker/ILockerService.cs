using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Services.Service.Locker
{
    public interface ILockerService
    {
        Task<List<LockerViewModel>> GetLocker(int? lockerId, int? cabinetId = null, int? companyId = null);
        Task<List<BookingTransactionsViewModel>> GetBookingTransactions(DateTime? startDate, DateTime? endDate, int? companyId = null, int? currentPage = null,
                                                                     int? pageSize = null, BookingTransactionStatus? bookingStatus = null);

        Task<List<BookingTransactionsViewModel>> GetUserBookingTransactions(string userKeyId, DateTime? startDate, DateTime? endDate, int? companyId = null, int? currentPage = null,
                                                             int? pageSize = null, BookingTransactionStatus? bookingStatus = null);

        Task<List<LockerDetailModel>> GetActiveLocker(int? lockerDetailId, int? companyId = null);
        Task<List<LockerDetailStatusModel>> GetActiveLockerStatus(int? lockerDetailId, int? companyId = null, int? cabinetId = null);
        Task<LockerBookingEntity> GetLockerBooking(int lockertransactionId);
        Task<ResponseValidityModel> SaveLocker(LockerDetailModel lockerDetailModel);
        Task<List<OTPModel>> SaveBookingLocker(List<LockerMobileBookingModel> lockerMobileBookingModel, string userKeyId);
        Task<List<OTPModel>> SaveLockerBooking(List<LockerManualBookingModel> lockerManualBookingModel);
        Task<ResponseValidityModel> SetLockerDetailActivation(int id, bool isDeleted);
        Task<List<AvailableLockerModel>> GetAvailableLocker(int? cabinetLocationId = null, int? lockerTypeId = null,
                                                            int? selectedMonth = null, int? selectedYear = null,
                                                            DateTime? startDate = null, DateTime? endDate = null,
                                                            bool isOrderByLockerDetailId = false, int? cabinetId = null,
                                                            int? currentPage = null, int? pageSize = null, int? positionId = null);
        Task<List<UpdatedAvailableLockerModel>> GetUpdatedAvailableLocker(int? cabinetLocationId, int? lockerTypeId,
                                                                         DateTime? startDate, DateTime? endDate,
                                                                         bool isOrderByLockerDetailId, int? cabinetId, int? currentPage,
                                                                         int? pageSize, int? positionId = null);
        Task<AvailableLockerListModel> GetAvailableLockerList(int? cabinetLocationId = null, int? lockerTypeId = null,
                                                            int? selectedMonth = null, int? selectedYear = null,
                                                            DateTime? startDate = null, DateTime? endDate = null,
                                                            bool isOrderByLockerDetailId = false, int? cabinetId = null,
                                                            int? currentPage = null, int? pageSize = null);

        Task<List<AvailableLockerModel>> GetAvailableDistinctLocker(int? cabinetLocationId = null, int? lockerTypeId = null,
                                                                    int? selectedMonth = null, int? selectedYear = null,
                                                                    DateTime? startDate = null, DateTime? endDate = null,
                                                                    bool isOrderByLockerDetailId = false, int? cabinetId = null,
                                                                    int? currentPage = null, int? pageSize = null);

        //Task<bool> UpdatedOTP(BookingTransactionStatus bookingStatus, string OTPCode, string qrCode, int lockerTransactionId);
        Task<OTPModel> UpdateOTP(string OTPCode);
        Task<OTPModel> ValidateOTP(string OTPCode, int bookingStatus, int cabinetId, int? companyId = null);
        Task<OTPModel> GenerateOTP(int lockerTransactionId, /*int cabinetId, int? companyId = null,*/ string userKeyId = null);

        Task<List<LockerBookingHistoryModel>> GetDropOffHistory(string userKeyId);
        Task<List<LockerBookingHistoryModel>> GetPickupHistory(string userKeyId);

        Task<List<LockerZoneModel>> GetLockerZones(string name = null, int? currentId = null);
        Task<ResponseValidityModel> SaveLockerZone(LockerZoneModel cabinetType);
        Task<ResponseValidityModel> DeleteLockerZone(int id);
        Task<List<ReassignedBookingLockerModel>> GetReassignedBookingLockerHistoryForAdmin(int? lockerDetailId = null, int? lockerTransactionsId = null, int? adminUserId = null, int? companyUserId = null, int? companyId = null);
        Task<ResponseValidityModel> ReassignBookingForAdmin(BookingLockerDetailModel param, string userKeyId);
        Task<List<ReassignedBookingLockerViewModel>> GetReassignedBookingLockerHistoryForCompanyUser(string userKeyId, int? lockerDetailId = null, int? lockerTransactionsId = null, int? companyUserId = null);

        Task<bool> SendBookingReceiptEmail(BookingReceiptModel model);
        Task<ResponseValidityModel> ReassignBookingForCompanyUser(BookingLockerDetailModel param, string userKeyId);
        Task<UserLockerBookingReportModel> GetBookingTransactionsReport(string userKeyId);


        ResponseValidityModel ValidateLockerBooking(ExtendLockerBookingModel model, string UserKeyId);
        Task<LockerBookingViewModel> GetLockerBookingByTransactionId(int lockerTransactionId);
        Task<UpdatedAvailableLockerModel> GetBookingUpdatedPrice(int lockerTransactionId, int lockerDetailId,
                                                                            DateTime startDate, DateTime endDate);
        Task<ResponseValidityModel> UpdateLockerBookingStatus(PostUpdateBookingStatusModel postUpdateBookingStatusModel);
        Task SendBookingNotification(int lockerTransactionId, NotificationType notificationType);
        Task<List<LockerBookingViewModel>> GetUserBookings(string userkeyId,
            BookingTransactionStatus? bookingStatus = null, DateTime? fromDate = null,
            DateTime? toDate = null, int? currentPage = null, int? pageSize = null, bool activeOnly = false);
        Task<List<LockerDetailModel>> GetUnavailableLockers(DateTime? startDate = null, DateTime? endDate = null);
        Task<ResponseValidityModel> CancelBooking(CancelBookingModel model, string UserKeyId);
        Task<int> GetActiveBookingCount(int lockerDetailId,
            DateTime storagePeriodStartDate,
            DateTime storagePeriodEndDate,
            int? lockerTransactionsId = null);
        Task<ResponseValidityModel> ExtendLockerBooking(ExtendLockerBookingModel model, string UserKeyId);

        Task<bool> ValidateAvailableLocker(int cabinetLocationId, int lockerTypeId, int positionId,
             int lcokerDetailId);
         Task<ResponseValidityModel> SelectLockerForBooking(int cabinetLocationId, int lockerTypeId, int positionId,
             DateTime storateStartDate, DateTime storageEndDate, string userKeyId, int lcokerDetailId);

        Task<ResponseValidityModel> ClearLockerForBooking(int cabinetLocationId, int lockerTypeId, int positionId,
             string userKeyId, int lcokerDetailId);

        void ScheduleUserNotifications(int id, DateTime storagePeriodEnd);

    }
}
