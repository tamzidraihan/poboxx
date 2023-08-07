using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class LockerDetailModel : BaseLockerModel<LockerDetailModel>
    {
        public string GetStatusCommand { get; set; }
        public string OpenCommand { get; set; }
        public int? CompanyId { get; set; }
        public string Size { get; set; }
        public string LockerTypeDescription { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CabinetLocationAddress { get; set; }
        public LockerStatus LockerStatus { get; set; }
        public List<LockerDetailBooking> Bookings { get; set; }
    }
    public class LockerDetailStatusModel : LockerDetailModel
    {
        public bool IsAvailable { get; set; }
      
        
    }
    public enum LockerStatus { Book = 0, Empty = 1, UnderRepair = 2 }
    public class LockerDetailBooking
    {
        public int LockerTransactionsId { get; set; }
        public DateTime? StoragePeriodStart { get; set; }
        public DateTime? StoragePeriodEnd { get; set; }
    }
}
