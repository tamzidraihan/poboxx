using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class LockerBookingHistoryModel  
    {
        public string LocationDescription { get; set; }
        public string Address { get; set; }
        public string LockerTypeDescription { get; set; }
        public int CabinetId { get; set; }
        public int CabinetLocationId { get; set; }
        public DateTime StoragePeriodStart { get; set; }
        public DateTime StoragePeriodEnd { get; set; }
        public DateTime DropOffDate { get; set; }
        public DateTime PickupDate { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public byte[] PackageImage { get; set; }
        public string PaymentReference { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateCreated { get; set; }
        public string LockerNumber { get; set; }
    }
}
