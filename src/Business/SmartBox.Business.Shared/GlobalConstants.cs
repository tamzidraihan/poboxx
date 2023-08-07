using NPOI.HSSF.Model;
using System.ComponentModel.Design.Serialization;

namespace SmartBox.Business.Shared
{
    public struct GlobalConstants
    {
        public struct NotificationParameters
        {

        }
        public struct MessageParameters
        {
            public const string Field = "{Field}";
            public const string NotificationType = "{NotificationType}";
            public const string Verb = "{Verb}";
            public const string Pin = "{PIN}";
            public const string Validity = "{VALIDITY}";
            public const string Refcode = "{REFCODE}";
            public const string PinCode = "{PINCODE}";
            public const string Type = "{Type}";
            public const string Name = "{Name}";
            public const string CabinetId = "{CabinetId}";
            public const string LockerNumber = "{LockerNumber}";
            public const string StartDate = "{StartDate}";
            public const string StartTime = "{StartTime}";
            public const string EndDate = "{EndDate}";
            public const string Recipient = "{Recipient}";
            public const string Id = "{Id}";
            public const string NoOfDays = "{NoOfDays}";
            public const string Location = "{Location}";
            public const string ContactNo = "{ContactNo}";
            public const string UserName = "{UserName}";
            public const string Password = "{Password}";
            public const string PlanType = "{PLAN_TYPE}";
            public const string HoursTxt = "{HOURS_TXT}";
            public const string LockerName = "{Locker_Name}";
            public const string LockerAddress = "{Locker_Address}";
            public const string ReceiverName = "{Receiver_Name}";
            public const string AppName = "{AppName}";
            public const string BookingTypeDescription = "{Booking_Type_Description}";
            public const string OtherInfo = "{OtherInfo}";
            public const string BookingDetail = "{BookingDetail}";
            public const string NewStoragePeriodEnd = "{NewStoragePeriodEnd}";
        }

        public struct MessageReplacement
        {
            public const string OTP = "OTP";
            public const string Email = "Email";
            public const string DropOff = "Drop-Off";
            public const string PickUp = "Pick-Up";
            public const string CompanyKeyId = "CompanyKeyId";
            public const string ParentCompanyId = "ParentCompanyId";
            public const string CabinetLocationId = "CabinetLocationId";
            public const string UserKeyId = "UserKeyId";
            public const string Username = "Username";
            public const string FirstnameOrLastname = "Firstname or Lastname";
            public const string Password = "Password";
            public const string Question = "Question";
            public const string Answer = "Answer";
            public const string CompanyId = "CompanyId";
            public const string RoleId = "RoleId";
            public const string RoleName = "Role Name";
            public const string FeedbackId = "FeedbackId";
            public const string UserId = "UserId";
            public const string RoleAndUserId = "Role And User";
            public const string RoleAndPermissionId = "Role And Permission";
            public const string UserRoleId = "UserRoleId";
            public const string PermissionId = "PermissionId";
            public const string PermissionName = "Permission Name";
            public const string BookingTransaction = "Booking Transaction";
            public const string ApplicationMessage = "Application Message";
            public const string ApplicationMessageId = "Application Message Id";
        }

        public struct ApplicationMessageNumber
        {
            public struct ErrorMessage
            {
                /* Error message */
                //public const int UserAlreadyActivated = -610;
                public const int IncorrectUsername = -1549;
               
                public const int AlreadyBooked = -610;
                public const int UserAlreadyActivated = -609;
                public const int UnchangeDefaultValue = -608;
                public const int InvalidForeignId = -607;
                public const int InvalidCustomField = -606;
                public const int NotExistingField = -605;
                public const int NotExistingMultipleField = -604;
                public const int NoItemSave = -603;
                public const int FieldExisting = -602;
                public const int FieldRequired = -601;
                public const int UnexpectedError = -600;
                public const int InvalidInput = -599;

                public const int EmailAlreadyExists = -545;
                public const int PhoneNoAlreadyExists = -546;
                public const int CabinetLocationAlreadyExists = -547;
                public const int PricingTypeAlreadyExists = -548;
                public const int PricingConfigMatrixAlreadyExists = -549;
                public const int PriceAndChargingAlreadyExists = -550;
                public const int LockerTypeAlreadyExists = -551;
                public const int LockerZoneAlreadyExists = -552;
                public const int CabinetAlreadyExists = -553;

                public const int RoleStillExists = -611;
                public const int LockerAssignmentError = -612;
                public const int BookingExtensionAlreadyExist = -613;


                public const int ExistingUser = -1;
                public const int NotExistingUser = -2;
                public const int WrongPassword = -3;
                public const int FailedNotification = -4;
                public const int OTPUnactivated = -5;
                public const int OTPExpired = -6;
                public const int InActiveuser = -7;
                public const int IncorrectOTP = -8;
                public const int OTPActivated = -9;
                public const int LocationNotFound = -10;
                public const int NoAvailableLocker = -11;
                public const int InvalidOTP = -12;
                public const int CompletedTransaction = -13;
                public const int PassDeadline = -14;
                public const int TransactionNotExisting = -15;
                public const int UnlockNotYetDue = -16;
                public const int ExceedNumberOfLocker = -17;
                public const int UnActivatedBackendUser = -18;
                public const int LockerTransactionIdForSubscription = -19;
                public const int LockerTransactionUnavailableForForSubscription = -20;
                public const int LockerExpiredForForSubscription = -21;
                public const int BookingStatusUpdate = -22;
                public const int BookingTransactionAlreadyComplete = -23;
                public const int CabinetLocationAssignedWithBooking = -24;
                public const int CabinetAssignedWithBooking = -25;
                public const int PricingMatrixDeleteConstraints = -26;
                public const int RoleDeleteConstraints = -27;
                public const int PermissionDeleteConstraints = -28;
                public const int IncorrectDropOffCode = -29;
                public const int IncorrectCollectCode = -30;
                public const int NotExistingUserName = -31;
                public const int PriceTypeDeleteConstraintsError = -32;
                public const int DeleteConstraintsError = -33;
                public const int LockerUnavailableAfterPayment = -34;
                public const int LockerUnavailableAfterPaymentLongMsg = -35;
            }

            public struct EmailMessage
            {
                public const int NewAdminUser = 999;
                public const int UpdateAdminUser = 998;
            }

            public struct InformationMessage
            {
                /* Information message */
                public const int LogInSuccess = 1;
                public const int LogOutSuccess = 2;
                public const int SMSOTP = 3;
                public const int AccountActivated = 4;
                public const int BookingCode = 5;
                public const int ValidOTP = 6;
                public const int TransactionCompleted = 7;
                public const int TransactionPickUp = 8;
                public const int DoorOpen = 9;

                public const int RecordAdded = 500;
                public const int RecordUpdated = 501;
                public const int ChildItemsNotExisting = 502;
                public const int NoRecords = 503;
                public const int RecordExists = 504;
                public const int ModelIsValid = 505;
                public const int NotificationSent = 506;
                public const int RecordDeactivated = 507;
                public const int RecordActivated = 508;
                public const int RecordDeleted = 509;
                public const int CompanyUserNotCreated = 510;
                public const int LockerSubscriptionExpireSoonShortMsg = 511;
                public const int LockerSubscriptionExpired = 512;
                public const int LockerSubscriptionExpiredShortMessage = 513;
                public const int MaintenanceReportNotificationSubject = 514;
                public const int MaintenanceReportNotificationMessage = 515;
                public const int CompanyLoginCredentials = 516;
                public const int IncuredCharges = 517;
                public const int OverStayCharges = 518;
                public const int LockerSubscriptionExpireTodayShortMsg = 519;
                public const int MaintenanceReportReminderNotificationSubject = 520;
                public const int MaintenanceReportReminderNotificationMessage = 521;
                public const int LockerSubscriptionExpireSoonLongMsg = 522;
                public const int LockerSubscriptionExpireTodayLongMsg = 523;
                public const int LockerSubscriptionExpiredLongMessage = 524;
                public const int LockerBookingExtendedMessageTitle = 525;
                public const int LockerBookingExtendedLongMessage = 526;
                public const int LockerBookingExtendedShortMessage = 527;
                public const int LockerBookingUnClaimedParcelLongMessage = 528;
                public const int LockerBookingUnClaimedParcelShortMessage = 529;
                public const int BookingDropOffPinLongMessage = 530;
                public const int BookingDropOffPinShortMessage = 531;
                public const int BookingDropOffSuccessLongMessage = 532;
                public const int BookingDropOffSuccessShortMessage = 533;
                public const int BookingCollectPinLongMessage = 534;
                public const int BookingCollectPinShortMessage = 535;
                public const int BookingCollectSuccessLongMessage = 536;
                public const int BookingCollectSuccessMessage = 537;
                public const int PaymentNotFound = 538;
                public const int UnpaidPayment = 539;
                public const int BookingExpired = 540;
                public const int EmailOrPhoneRequired = 541;
                public const int ForgetPasswordSms = 542;
                public const int ForgetPasswordEmail = 543;
                public const int PasswordDelivered = 544;
                public const int CodeExpired = 547;
                public const int CodeNotLiveYet = 548;
                public const int IncorrectUsername = 549;
                public const int LockerReassignedShort = 549;
                public const int LockerReassignedLong = 550;
                public const int LockerReassignedSuccess = 551;
                public const int MultiBookingDropOffCode = 552;

            }
        }

        public struct Roles
        {
            public const string Admin = "Admin";
            public const string NonAdministrator = "NonAdministrator";
            public const string BackendUser = "BackendUser";
            public const string BackendUserAdmin = "BackendUserAdmin";
        }

        public struct PaymongoPayment
        {
            public const string Gcash = "gcash";
            public const string GrabPay = "grab_pay";
        }

        public struct PaymongoCurrency
        {
            public const string PHP = "PHP";
        }

        public struct PayamayaStatus
        {
            public const string PaymentSuccess = "PAYMENT_SUCCESS";
            public const string PaymentFailed = "PAYMENT_FAILED";
            public const string PaymentExpired = "PAYMENT_EXPIRED";
            public const string AuthFailed = "AUTH_FAILED";
        }

        //public struct DatabaseTables
        //{
        //    public const string ApplicationMessage = "application_messages";
        //}

        //public struct QueryParameters
        //{
        //    public const string ApplicationMessageId = "@applicationMessagId";
        //    public const string EmailAddress = "@emailAddress";
        //    public const string PhoneNumber = "@phoneNumber";
        //}

        //public struct MessageParameters
        //{
        //    public const string Field = "{Field}";
        //    public const string NotificationType = "{NotificationType}";
        //}
    }
}

