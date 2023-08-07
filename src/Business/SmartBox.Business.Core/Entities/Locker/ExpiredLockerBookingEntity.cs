using SmartBox.Business.Core.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Core.Entities.Locker
{
    public class ActiveLockerBookingEntity
    {
        public int LockerTransactionsId { get; set; }
        public int LockerDetailId { get; set; }
        public DateTime StoragePeriodStart { get; set; }
        public DateTime StoragePeriodEnd { get; set; }
        public DeviceType DeviceType { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string LockerNumber { get; set; }
        public string CabinetLocationDescription { get; set; }
        public bool IsSubscriptionBooking { get; set; }
        public string PaymentReference { get; set; }
        public BookingTransactionStatus BookingStatus { get; set; }
        public string DropOffCode { get; set; }
        public string PickUpCode { get; set; }
        public string UserFirstName { get; set; }
        public int AccessPlan { get; set; }

    }
}
