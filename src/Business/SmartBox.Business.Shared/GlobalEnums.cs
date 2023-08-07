namespace SmartBox.Business.Shared
{
    public class GlobalEnums
    {
        public enum AppStatusType
        {
            Information = 1,
            Warning = 2,
            Error = 3
        }
        public enum WalletTransactionType
        {
            Pending = 1,
            Cancelled = 2,
            Rejected = 3
        }



        public enum PaymentMethod
        {
            CreditCard = 1,
            BankTransfer = 2,
            PaymentCenter_EWallet = 3,
            TopUp = 4,
            Gcash = 5
        }

        public enum PaymentMedium
        {
            Paymongo = 1,
            Paymaya = 2
        }
        public enum BookingTransactionStatus
        {
            ForDropOff = 1,
            ForPickUp = 2,
            Confiscated = 3,
            Completed = 4,
            Reassigned = 5,
            Cancelled = 6,
            Expired = 7
        }
        public enum NotificationType
        {
            ForDropOffPin = 1,
            ForDropOffSuccessNotification = 2,
            ForCollectPin = 3,
            ForCollectSuccessNotification = 4,
            ForBookingExtentionSuccessNotification = 5,
            ForLockerReassignmentNotification = 6,
            ForBookingCancellation = 7,
            
        }
        public enum FranchiseFeedbackQuestionType
        {
            DropOff = 1,
            Pickup = 2,
            Franchise = 3
        }

        public enum BookingPlan
        {
            SingleAcces =1,
            MultiAccess =2
        }
    }
}
