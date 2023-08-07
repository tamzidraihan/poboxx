using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Booking
{
    public class BookingReceiptModel
    {
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; }
        public string UserEmail { get; set; }
        public List<BookingReceiptDetailModel> Detail { get; set; }

    }
    public class BookingReceiptDetailModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Location { get; set; }
        public string LockerSize { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverMobileNo { get; set; }
        public string referenceNo { get; set; }
        public decimal TotalPrice { get; set; }
        public Dictionary<string, decimal> PriceBreakdown { get; set; }
    }
}
