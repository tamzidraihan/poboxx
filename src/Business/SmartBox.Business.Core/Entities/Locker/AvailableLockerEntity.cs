using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Locker
{
    public class AvailableLockerEntity : LockerDetailEntity
    {
        public int TotalRecordCount { get; set; }
        public new string CabinetLocationDescription { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Rating { get; set; }

        public decimal Price { get; set; }
        public string UserKeyId { get; set; }
        public new DateTime StoragePeriodStart { get; set; }
        public new DateTime StoragePeriodEnd { get; set; }
        public new int? BookingStatus { get; set; }
        public int OverstayPeriod { get; set; }
        public string PricingType { get; set; }
        public decimal StoragePrice { get; set; }
        public decimal OverstayCharge { get; set; }
        public decimal MultiAccessStoragePrice { get; set; }
    }
    public class UpdatedAvailableLockerEntity : LockerDetailEntity
    {
        public int TotalRecordCount { get; set; }
        public new string CabinetLocationDescription { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Rating { get; set; }

        public decimal Price { get; set; }
        public string UserKeyId { get; set; }
        public new DateTime StoragePeriodStart { get; set; }
        public new DateTime StoragePeriodEnd { get; set; }
        public new int? BookingStatus { get; set; }
        public int OverstayPeriod { get; set; }
        public string PricingType { get; set; }
        public decimal StoragePrice { get; set; }
        public decimal OverstayCharge { get; set; }
        public decimal MultiAccessStoragePrice { get; set; }
    }
}
