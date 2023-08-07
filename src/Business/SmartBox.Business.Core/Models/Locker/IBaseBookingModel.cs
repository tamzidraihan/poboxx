using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public interface IBaseBookingModel<T>
    {
        //public int LockerDetailId { get; set; }
        public int CabinetLocationId { get; set; }
        public int LockerTypeId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public int PaymentMethodId { get; set; }    
        public string PaymentReference { get; set; }
        public DateTime StoragePeriodStart { get; set; }
        public DateTime StoragePeriodEnd { get; set; }
    }
}
