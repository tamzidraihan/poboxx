using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.User;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Infrastructure.Data.Repository.Locker
{
    public interface ILockerRepository : IGenericRepositoryBase<LockerDetailEntity>
    {
        Task<List<LockerDetailEntity>> GetActiveLocker(int? lockerDetailId, int? cabinetId = null, int? companyId = null);
        Task<List<LockerDetailEntity>> GetActiveLockerStatus(int? lockerDetailId = null, int? cabinetId = null, int? companyId = null);
        Task<List<LockerDetailEntity>> GetLockerDetail(int? lockerDetailId = null, string lockerNumber = null, int? cabinetId = null, int? companyId = null);
        Task<List<AvailableLockerEntity>> GetAvailableLocker(int? cabinetLocationId, int? lockerTypeId, int? selectedMonth,
                                                             int? selectedYear, DateTime? startDate, DateTime? endDate,
                                                             bool isOrderByLockerDetailId, int? cabinetId,
                                                             int? currentPage, int? pageSize, int? positionId = null);
        Task<List<UpdatedAvailableLockerEntity>> GetUpdatedAvailableLocker(int? cabinetLocationId, int? lockerTypeId,
                                                                         DateTime? startDate, DateTime? endDate,
                                                                         bool isOrderByLockerNumber, int? cabinetId,
                                                                         int? currentPage, int? pageSize, int? positionId = null);
        Task<List<BookingTransactionsViewModel>> GetBookingTransactions(DateTime? startDate, DateTime? endDate, int? companyId = null, int? currentPage = null,
                                                                    int? pageSize = null, BookingTransactionStatus? bookingStatus = null);
        Task<List<BookingTransactionsViewModel>> GetUserBookingTransactions(string userKeyId, DateTime? startDate, DateTime? endDate, int? companyId = null,
                                                                       int? currentPage = null, int? pageSize = null, BookingTransactionStatus? bookingStatus = null);

        Task<int> SaveLocker(LockerDetailEntity lockerDetailEntity);
        Task<int> SaveLockerBooking(LockerBookingEntity lockerBookingEntity);
        Task<int> SetLockerDetailActivation(int id, bool isAvailable);
        //Task<bool> UpdateLockerDetail(int lockerDetailId, int lockerTransactionId, string qrCode, string pinCode, int isAvailable);
        Task<int> UpdatedOTP(BookingTransactionStatus bookingStatus, string OTPCode, string qrCode, int lockerTransactionId, bool isSubscriptionBooking = false);
        Task<LockerBookingEntity> GetOTP(string OTPCode);
        Task<LockerBookingEntity> GetOTPStatus(string OTPCode);
        Task<LockerBookingEntity> GetDropOffOTP(string OTPCode);
        Task<LockerBookingEntity> GetLockerBooking(int lockertransactionId);
        Task<List<LockerBookingHistoryModel>> GetPickupHistory(string userKeyId);
        Task<List<LockerBookingHistoryModel>> GetDropOffHistory(string userKeyId);

        Task<int> SaveLockerZone(LockerZoneEntity lockerZoneEntity);
        Task<List<LockerZoneEntity>> GetLockerZones(string name = null, int? currentId = null);
        Task<List<ReassignedBookingLockerEntity>> GetReassignedBookingLockerHistory(int? lockerDetailId = null, int? lockerTransactionsId = null, int? adminUserId = null, int? companyUserId = null, int? companyId = null);
        Task<int> ReassignBooking(LockerBookingEntity existing, BookingLockerDetailModel model, int lockerDetailId, string otpCode, string otpQRCode, int? adminUserId = null, int? companyUserId = null);
        Task<bool> IsValidTransaction(int lockerTransactionsId, int companyId);
        Task<UserLockerBookingReportModel> GetBookingTransactionsReport(string userKeyId);
        Task<List<ActiveLockerBookingEntity>> GetExpiredLockerBookings(int? lockertransactionId = null);
        Task UpdateNotifiedLockerBookings(List<int> lockerTransactionIds);
        Task<List<ActiveLockerBookingEntity>> GetAtiveLockerBookingDetail(int LockerTransactionsId);
        Task<int> ActiveBookingsCount(int lockerDetailId, DateTime fromDate, DateTime toDate, int? excludeLockerTransactionId = null);
        Task<UpdatedAvailableLockerEntity> GetBookingUpdatedPrice(int lockerTransactionId, int lockerDetailId, DateTime endDate);
        Task<LockerBookingEntity> GetLockerBookingByTransactionId(int lockerTransactionId);
        Task<List<LockerBookingPaymentDetail>> GetUserBookings(string userkeyId,
            BookingTransactionStatus? bookingStatus = null, DateTime? fromDate = null,
            DateTime? toDate = null, int? currentPage = null, int? pageSize = null, bool activeOnly = false);
        Task<int> UpdateBookingStatus(int lockerTransactionId, int bookingStatus);
        Task<List<LockerDetailEntity>> GetUnavailableLockers(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> CancelLockerBooking(CancelBookingModel cancelBookingModel, LockerBookingEntity existingBooking, string userKeyId);
        Task<int> Delete(int id);
        Task<int> ExtendLockerBooking(ExtendLockerBookingModel extendLockerBookingModel, LockerBookingEntity existingBooking, string userKeyId);
        Task<LockerBookingEntity> GetLockerBookingDetail(int LockerTransactionsId);

        Task<int> ValidateAvailableLocker(int cabinetLocationId, int lockerTypeId, int positionId,
         int lcokerDetailId);
        Task<int> SelectLockerForBooking(int cabinetLocationId, int lockerTypeId, int positionId,
            DateTime storateStartDate, DateTime storageEndDate, string userKeyId, int lcokerDetailId);

        Task<int> ClearLockerForBooking(int cabinetLocationId, int lockerTypeId, int positionId,
            string userKeyId, int lcokerDetailId);
    }
}
