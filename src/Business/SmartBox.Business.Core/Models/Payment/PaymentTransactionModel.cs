using SmartBox.Business.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Payment
{
    public class PaymentTransactionModel
    {
        public int PaymentGatewayTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
      
        //Required In case of Cancelling the booking
        public PaymentInternalType? InternalType { get; set; }
        public int? LockerTransactionsId { get; set; }
        public PaymentInternalStatus? InternalStatus { get; set; }
        //Required in case of ExtendBooking
        public DateTime? NewStoragePeriodEndDate { get; set; }
        public string Error { get; set; }

    }
}
