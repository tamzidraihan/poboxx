using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Core.Models.Booking
{
    public class PostUpdateBookingStatusModel
    {
        public int LockerTransactionId { get; set; }
        public BookingTransactionStatus BookingStatus { get; set; }
    }
}
