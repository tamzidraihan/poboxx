using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Core.Models.Locker
{
    public class AvailableLockerModel : AvailableLockerDetail
    {
        public string CabinetLocationDescription { get; set; }
        public DateTime StoragePeriodStart { get; set; }
        public DateTime StoragePeriodEnd { get; set; }
        //public DateTime DropOffDate { get; set; }
        //public string DropOffCode { get; set; }
        //public string DropOffQRCode { get; set; }
        //public DateTime PickupDate { get; set; }
        //public string PickUpCode { get; set; }
        //public string PickUpQRCode { get; set; }
        public int OverstayPeriod { get; set; }
        public string PricingType { get; set; }
        public decimal OverstayCharge { get; set; }
        public decimal StoragePrice { get; set; }
        public BookingTransactionStatus? BookingStatus { get; set; }
    }
    public class UpdatedAvailableLockerModel : AvailableLockerDetail
    {
        public string CabinetLocationDescription { get; set; }
        public int OverstayPeriod { get; set; }
        public string PricingType { get; set; }
        public decimal OverstayCharge { get; set; }
        public decimal OverstayChargeValue { get; set; }
        public decimal StoragePrice { get; set; }
        public decimal MultiAccessStoragePrice { get; set; }
    }
}
