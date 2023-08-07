using SmartBox.Business.Core.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Core.Models.Locker
{
    public class LockerBookingViewModel : LockerMobileBookingModel
    {
        public string UserKeyId { get; set; }
        public int LockerTransactionsId { get; set; }
        public string LockerNumber { get; set; }
        public string DropOffCode { get; set; }
        public string DropOffQRCode { get; set; }
        public string PickUpCode { get; set; }
        public string PickUpQRCode { get; set; }
        public BookingTransactionStatus? BookingStatus { get; set; }
        public bool IsSubscriptionBooking { get; set; }
        public DateTime? CancelledDate { get; set; }
        public int CabinetId { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CabinetLocationAddress { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public List<PaymentTransactionModel> Payments { get; set; }
    }
}
