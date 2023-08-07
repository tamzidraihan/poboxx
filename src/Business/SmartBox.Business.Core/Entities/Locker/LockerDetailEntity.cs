using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Locker
{
    public class LockerDetailEntity
    {
        public int LockerDetailId { get; set; }
        public int CabinetId { get; set; }
        public string Size { get; set; }
        public int LockerTypeId { get; set; }
        public string LockerTypeDescription { get; set; }
        public bool IsDeletedLockerType { get; set; }
        public string LockerNumber { get; set; }
        public string BoardNumber { get; set; }
        public string GetStatusCommand { get; set; }
        public string OpenCommand { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsBookingExist { get; set; }
        public int? BookingStatus { get; set; }
        public int? BookingPlan { get; set; }
        public int LockerTransactionsId { get; set; }
        public string QRCode { get; set; }
        public string PinCode { get; set; }
        public int PositionId { get; set; }
        public int? CompanyId { get; set; }
        public int CabinetLocationId { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CabinetLocationAddress { get; set; }
        public DateTime? StoragePeriodStart { get; set; }
        public DateTime? StoragePeriodEnd { get; set; }

    }
}
