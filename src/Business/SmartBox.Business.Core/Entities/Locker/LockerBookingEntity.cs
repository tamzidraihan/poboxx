using SmartBox.Business.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Core.Entities.Locker
{
    public class LockerBookingEntity
    {
        public int LockerTransactionsId { get; set; }
        public string UserKeyId { get; set; }
        public int CompanyId { get; set; }
        public int LockerDetailId { get; set; }
        public DateTime StoragePeriodStart { get; set; }
        public DateTime StoragePeriodEnd { get; set; }
        public DateTime? NewStoragePeriodEndDate { get; set; }
        public string SenderName { get; set; }
        public string SenderMobile { get; set; }
        public string SenderEmailAddress { get; set; }
        public byte[] PackageImage { get; set; }
        public DateTime? DropOffDate { get; set; }
        public string DropOffCode { get; set; }
        public string DropOffQRCode { get; set; }
        public DateTime? PickupDate { get; set; }
        public string PickUpCode { get; set; }
        public string PickUpQRCode { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentReference { get; set; }
        public int BookingStatus { get; set; }
        public int AccessPlan { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string LockerNumber { get; set; }
        public string BoardNumber { get; set; }
        public string OpenCommand { get; set; }
        public string GetStatusCommand { get; set; }
        public bool IsSubscriptionBooking { get; set; }
        public string CabinetLocationDescription { get; set; }


        public int? ReassignedByAdminUser { get; set; }
        public int? ReassignedByCompanyUser { get; set; }        
        public bool IsNotified { get; set; }
        public DateTime NotificationDate { get; set; }        
        public DateTime? CancelledDate { get; set; }                
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        
    }
    public class LockerBookingPaymentDetail
    {
        public int LockerTransactionsId { get; set; }
        public int LockerDetailId { get; set; }
        public string LockerNumber { get; set; }
        public string UserKeyId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string DropOffCode { get; set; }
        public string DropOffQRCode { get; set; }
        public string PickUpCode { get; set; }
        public string PickUpQRCode { get; set; }
        public decimal TotalPrice { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentReference { get; set; }
        public BookingTransactionStatus? BookingStatus { get; set; }
        public DateTime StoragePeriodStart { get; set; }
        public DateTime StoragePeriodEnd { get; set; }
        public bool IsSubscriptionBooking { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public int CabinetId { get; set; }
        public int CabinetLocationId { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CabinetLocationAddress { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public PaymentInternalType? InternalType { get; set; }
        public PaymentInternalStatus? InternalStatus { get; set; }
        public string TransactionId { get; set; }
        public int BookingPlan { get; set; }
    }
}
