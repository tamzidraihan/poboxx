using System;

namespace SmartBox.Business.Core.Entities.Payment
{
    public class PaymentTransactionEntity
    {
        public int PaymentGatewayTransactionId { get; set; }
        public string TransactionId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public PaymentInternalType? InternalType { get; set; }
        public int? LockerTransactionsId { get; set; }
        public PaymentInternalStatus? InternalStatus { get; set; }
        public DateTime? NewStoragePeriodEndDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

    }
    public enum PaymentInternalType
    {
        NewBooking = 0, CancelingBooking = 1, ExtendingBooking = 2
    }
    public enum PaymentInternalStatus
    {
        Pending = 0, Paid = 1
    }
}
