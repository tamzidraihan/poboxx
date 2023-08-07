namespace SmartBox.Business.Shared
{
    public struct GlobalDatabaseConstants
    {
        public struct DatabaseTables
        {
            public const string ApplicationMessage = "application_messages";
            public const string ApplicationSettings = "application_settings";
            public const string Users = "users";
            public const string Cabinets = "cabinets";
            public const string CabinetLocation = "cabinet_locations";
            public const string LockerType = "locker_types";
            public const string LockerDetail = "locker_detail";
            public const string LockerBooking = "locker_bookings";
            public const string PaymentMethod = "payment_methods";
            public const string AdminUser = "admin_users";
            public const string PaymongoTransaction = "paymongo_transaction";
            public const string EmailMessage = "email_message";
            public const string PaymentTransaction = "payment_transaction";
            public const string Company = "company";
            public const string Feedback = "feedback";
            public const string ParentCompany = "parent_company";
            public const string CabinetTypes = "cabinet_types";
            public const string LockerZones = "locker_zones";
            public const string AnnouncementType = "announcement_type";
            public const string PricingType = "pricing_type";
            public const string PricingMatrix = "pricing_matrix";
            public const string OverstayPriceConfiguration = "overstay_price_configuration";
            public const string PromoAnnouncement = "promo_announcement";
            public const string UserSubscription = "user_subscription";
            public const string MaintenanceInspectionTesting = "maintenance_inspection_testing";
            public const string MaintenanceReasonType = "maintenance_reason_type";
            public const string Charges = "charges";
            public const string CompanyUsers = "company_users";
            public const string CleanlinessReport = "cleanliness_report";
            public const string LockerPictures = "locker_pictures";
            public const string Role = "roles";
            public const string UserRole = "user_roles";
            public const string Permission = "permissions";
            public const string RolePermission = "role_permission";
            public const string UserSubscriptionBilling = "user_subscription_billing";
            public const string UserToken = "user_token";
            public const string MessageLog = "message_log";
            public const string CompanyCabinet = "company_cabinet";
            public const string ReassignedBookingLockerHistory = "reassigned_booking_locker_history";
            public const string FranchiseFeedbackQuestion = "franchise_feedback_question";
            public const string FranchiseFeedbackAnswer = "franchise_feedback_answer";
            public const string LockerBookingHistory = "locker_booking_history";
            public const string UserFavouritesLocations = "user_favourites_cabinet_locations";
            public const string PromoAndDiscounts = "promo_and_discounts";
            public const string Ads = "Ads";
        }

        public struct Views
        {
            public const string BookingLockerDetail = "vw_locker_booking_locker";
            public const string ActiveCabinetAndLocation = "vw_active_cabinet";
            public const string ActiveLockers = "vw_active_locker";
            public const string ActiveLockersStatus = "vw_active_locker_status";
            public const string ActiveLockerType = "vw_active_locker_type";
            public const string ActiveCabinetLocation = "vw_active_cabinet_location";
            public const string CabinetLocation = "vw_cabinet_location";
            public const string Locker = "vw_locker";
            public const string CabinetWithLocation = "vw_cabinet_with_location";
            public const string CompanyWithLocation = "vw_company_cabinet_location";
            public const string CompanyAndCabinet = "vw_company_cabinet";
            public const string BookingHistory = "vw_booking_history";
            public const string PricingType = "vw_pricing_type";
            public const string PricingMatrix = "vw_pricing_matrix";
            public const string OverstayPriceConfiguration = "vw_overstay_price_configuration";
            public const string AnnouncementType = "vw_announcement_type";
            public const string PromoAnnouncement = "vw_promo_announcement";
            public const string UserSubscription = "vw_user_subscription";
            public const string MaintenanceInspectionTesting = "vw_maintenance_inspection_testing";
            public const string Charges = "vw_charges";
            public const string MaintenanceReasonType = "vw_maintenance_reason_type";
            public const string CleanlinessReport = "vw_cleanliness_report";
            public const string UserSubscriptionBilling = "vw_user_subscription_billing";
            public const string UserToken = "vw_user_token";
            public const string UserSubscriptionExpiration = "vw_user_subscripion_expiration ";
            public const string MessageLog = "vw_message_log";
            public const string ReassignedBookingLockerHistory = "vw_reassigned_booking_locker_history";
            public const string RolePermission = "vw_role_permission ";
        }

        public struct StoredProcedures
        {
            public const string GetBookingsCount = "sp_GetBookingsCount";
            public const string GetDropOffCount = "sp_GetDropOffCount";
            public const string GetLocationsCount = "sp_GetLocationsCount";
            public const string GetDeliveriesCount = "sp_GetDeliveriesCount";
            public const string GetRecentBookings = "sp_GetRecentBookings";
            public const string GetAvailableLockers = "sp_GetAvailableLockers";
            public const string GetUpdatedAvailableLockers = "sp_GetUpdatedAvailableLockers";
            public const string GetMostBookedLockers = "sp_GetMostBookedLockers";
            public const string GetRevenue = "sp_GetRevenue";
            public const string GetTodayNotifications = "sp_GetTodayNotifications";
            public const string GetYesterdayNotifications = "sp_GetYesterdayNotifications";
            public const string GetBookingTransactions = "sp_GetBookingTransactions";
            public const string GetUnsubmittedCleanlinessReport = "sp_UnsubmittedCleanlinessReport";
            public const string GetAllFranchiseFeedbackQuestion = "sp_GetAllFranchiseFeedbackQuestion";
            public const string GetAllFranchiseFeedbackAnswer = "sp_GetAllFranchiseFeedbackAnswer";
            public const string GetAllFeedback = "sp_GetAllFeedbacks";
            public const string ValidateBookingTransaction = "sp_ValidateBookingTransaction";
            public const string UserMontlyReport = "sp_UserMonthlyReport";
            public const string UserAnuallyReport = "sp_UserAnuallyReport";
            public const string MessageLog = "sp_GetMessageLog";
            public const string ExpiredLockerBookings = "sp_ExpiredLockerBookings";
            public const string ActiveLockerBooking = "sp_ActiveLockerBooking";
            public const string ActiveBookingCount = "sp_ActiveBookingCount";
            public const string GetCleanlinessReport = "sp_GetCleanlinessReport";
            public const string GetBookingUpdatedPrice = "sp_GetBookingUpdatedPrice";
            public const string GetLogs = "sp_GetLogs";
            public const string GetUnavailableLockers = "sp_GetUnavailableLockers";
            public const string BookingDetailWithPayments = "sp_BookingDetailWithPayments";
            public const string LockerBookingDetail = "sp_LockerBookingDetail";
            public const string GetAllPromoAndDiscounts = "sp_GetAllPromoAndDiscounts";
            public const string GetAllAds = "sp_GetAllAds";
        }

        public struct QueryParameters
        {
            public const string ApplicationMessageId = "@applicationMessagId";
            public const string EmailAddress = "@emailAddress";
            public const string PhoneNumber = "@phoneNumber";
            public const string UserKeyId = "@userKeyId";
            public const string IsActivated = "@IsActivated";
            public const string CabinetLocationId = "@cabinetLocationId";
            public const string CabinetId = "@cabinetId";
            public const string LockerTypeId = "@lockerTypeId";
            public const string LockerTransactionId = "@lockerTransactionId";
            public const string LockerDetailId = "@lockerDetailId";
            public const string SelectedMonth = "@selectedMonth";
            public const string SelectedYear = "@selectedYear";
            public const string StartDate = "@startDate";
            public const string EndDate = "@endDate";
            public const string IsOrderByLockerNumber = "@isOrderByLockerNumber";
            public const string CurrentPage = "@currentPage";
            public const string PageSize = "@pageSize";
            public const string RoleId = "@roleId";
            public const string FeedbackId = "@FeedbackId";
            public const string RoleName = "@roleName";
            public const string Name = "@name";
            public const string UserId = "@userId";
            public const string UserRoleId = "@userRoleId";
            public const string PermissionId = "@permissionId";
            public const string RolePermissionId = "@rolePermissionId";
            public const string NumOfRow = "@numOfRow";
            public const string EmailMessageId = "@emailMessageId";
            public const string LockerNumber = "@lockerNumber";
            public const string CompanyId = "@companyId";
            public const string CompanyKeyId = "@companyKeyId";
            public const string IsActive = "@isActive";
            public const string IsDeleted = "@isDeleted";
            public const string ParentCompanyKeyId = "@parentCompanyKeyId";
            public const string ParentCompanyName = "@parentCompanyName";
            public const string ParentCompanyId = "@parentCompanyId";
            public const string PromoAndDiscountsId = "@promoAndDiscountsId";
            public const string AdsId = "@adsId";
            public const string Email = "@email";
            public const string Username = "@Username";
            public const string IsSystemGenerated = "@isSystemGenerated";
            public const string LockerTransactionsId = "@lockerTransactionsId";
            public const string OldLockerDetailId = "@oldLockerDetailId";
            public const string NewLockerDetailId = "@newLockerDetailId";
            public const string ReassignedByAdminUser = "@reassignedByAdminUser";
            public const string ReassignedByCompanyUser = "@reassignedByCompanyUser";
            public const string FranchiseFeedbackQuestionEntityId = "@Id";
            public const string FranchiseFeedbackAnswerEntityId = "@Id";
            public const string Question = "@question";
            public const string Answer = "@answer";
            public const string DropOffCount = "@DropOffCount";
            public const string PickUpCount = "@PickUpCount";
            public const string ConfiscatedCount = "@ConfiscatedCount";
            public const string CompletedCount = "@CompletedCount";
            public const string BookingStatus = "@bookingStatus";
            public const string OtherBookingStatus = "@otherbookingStatus";
            public const string userKey = "@userKey";
            public const string Status = "@status";
            public const string PositionId = "@positionId";
            public const string DaysAllowed = "@daysAllowed";
            public const string Month = "@setMonth";
            public const string Year = "@setYear";
            public const string PageNumber = "@pageNumber";
            public const string StatusId = "@statusId";
            public const string LogType = "@logType";
            public const string Search = "@search";

        }
    }
}
