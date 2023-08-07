using AutoMapper;
using SmartBox.Business.Core.Entities.ApplicationSetting;
using SmartBox.Business.Core.Entities.AppMessage;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Entities.Company;
using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Entities.Payment;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Core.Models.ApplicationSetting;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Email;
using SmartBox.Business.Core.Models.Company;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.MessageModel;
using SmartBox.Business.Core.Models.Payment;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Core.Models.ParentCompany;
using SmartBox.Business.Core.Entities.ParentCompany;
using SmartBox.Business.Core.Entities.Pricing;
using SmartBox.Business.Core.Models.Pricing;
using SmartBox.Business.Core.Entities.Announcement;
using SmartBox.Business.Core.Models.Announcement;
using SmartBox.Business.Core.Entities.Maintenance;
using SmartBox.Business.Core.Models.Maintenance;
using SmartBox.Business.Core.Entities.CompanyUser;
using SmartBox.Business.Core.Models.CompanyUser;
using SmartBox.Business.Core.Entities.Report;
using SmartBox.Business.Core.Models.Report;
using SmartBox.Business.Core.Models.Roles;
using SmartBox.Business.Core.Entities.Role;
using SmartBox.Business.Core.Models.UserRole;
using SmartBox.Business.Core.Entities.UserRole;
using SmartBox.Business.Core.Models.RolePermission;
using SmartBox.Business.Core.Entities.RolePermission;
using SmartBox.Business.Core.Models.Permission;
using SmartBox.Business.Core.Entities.Permission;
using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Entities.Logs;
using SmartBox.Business.Core.Models.Logger;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.FranchiseFeedbackQuestion;
using SmartBox.Business.Core.Entities.FranchiseFeedbackQuestion;
using SmartBox.Business.Core.Models.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Entities.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Entities.Email;
using SmartBox.Business.Core.Models.AppMessage;
using SmartBox.Business.Core.Entities.PromoAndDiscounts;
using SmartBox.Business.Core.Models.PromoAndDiscounts;

namespace SmartBox.Business.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserEntity, UserFormModel>();
            CreateMap<UserFormModel, UserEntity>();
            CreateMap<UserEntity, UserModel>();
            CreateMap<UserModel, UserEntity>();

            CreateMap<UserDetailModel, UserEntity>();
            CreateMap<UserEntity, UserDetailModel>();

            CreateMap<AdminUserModel, AdminUserEntity>();
            CreateMap<AdminUserEntity, AdminUserModel>();

            CreateMap<CabinetModel, CabinetEntity>();
            CreateMap<CabinetEntity, CabinetModel>();
            CreateMap<CabinetLocationEntity, CabinetLocationModel>();
            CreateMap<CabinetLocationModel, CabinetLocationEntity>();

            CreateMap<CabinetLocationEntity, CabinetLocationViewModel>();
            CreateMap<CabinetEntity, CabinetViewModel>();

            CreateMap<LockerTypeEntity, LockerTypeModel>();
            CreateMap<LockerTypeModel, LockerTypeEntity>();

            CreateMap<UserTokenEntity, UserTokenModel>();
            CreateMap<UserTokenModel, UserTokenEntity>();

            CreateMap<MessageLogEntity, MessageLogModel>();
            CreateMap<MessageLogModel, MessageLogEntity>();

            CreateMap<UserSubscriptionBillingEntity, UserSubscriptionBillingModel>();
            CreateMap<UserSubscriptionBillingModel, UserSubscriptionBillingEntity>();

            CreateMap<LockerDetailModel, LockerDetailEntity>();
            CreateMap<LockerDetailEntity, LockerDetailModel>();
            CreateMap<LockerDetailEntity, LockerDetailStatusModel>();


            CreateMap<CompanyCabinetEntity, CompanyCabinetViewModel>();
            CreateMap<CompanyCabinetModel, CompanyCabinetEntity>();

            CreateMap<ChargesModel, ChargesEntity>();
            CreateMap<ChargesEntity, ChargesViewModel>();

            CreateMap<LockerDetailEntity, LockerViewModel>();

            CreateMap<ReassignedBookingLockerEntity, ReassignedBookingLockerModel>();
            CreateMap<ReassignedBookingLockerEntity, ReassignedBookingLockerViewModel>();

            CreateMap<LockerTypeEntity, LockerTypeViewModel>();
            CreateMap<LockerTypeViewModel, LockerTypeEntity>();

            CreateMap<CleanlinessReportEntity, CleanlinessReportViewModel>();
            CreateMap<CleanlinessReportModel, CleanlinessReportEntity>();

            CreateMap<LockerPictureModel, LockerPictureEntity>();


            CreateMap<AvailableLockerEntity, AvailableLockerModel>();
            CreateMap<AvailableLockerModel, AvailableLockerEntity>();

            CreateMap<UpdatedAvailableLockerEntity, UpdatedAvailableLockerModel>();


            CreateMap<PriceAndChargingEntity, PriceAndChargingModel>();
            CreateMap<PriceAndChargingModel, PriceAndChargingEntity>();

            CreateMap<PricingMatrixConfigEntity, PriceMatrixConfigModel>();
            CreateMap<PriceMatrixConfigModel, PricingMatrixConfigEntity>();

            CreateMap<PricingTypeEntity, PricingTypeModel>();
            CreateMap<PricingTypeModel, PricingTypeEntity>();

            CreateMap<MaintenanceReasonTypeEntity, MaintenanceReasonTypeModel>();
            CreateMap<MaintenanceReasonTypeModel, MaintenanceReasonTypeEntity>();

            CreateMap<UserSubscriptionEntity, UserSubscriptionViewModel>();
            CreateMap<UserSubscriptionModel, UserSubscriptionEntity>();

            CreateMap<MaintenanceInspectionTestingEntity, MaintenanceInspectionTestingViewModel>();
            CreateMap<MaintenanceInspectionTestingModel, MaintenanceInspectionTestingEntity>();

            CreateMap<AnnouncementTypeEntity, AnnouncementTypeModel>();
            CreateMap<AnnouncementTypeModel, AnnouncementTypeEntity>();

            CreateMap<PromoAnnouncementEntity, PromoAnnouncementModel>();
            CreateMap<PromoAnnouncementModel, PromoAnnouncementEntity>();
            
            CreateMap<PromoAndDiscountsEntity, PromoAndDiscountsModel>();
            CreateMap<PromoAndDiscountsModel, PromoAndDiscountsEntity>();
            
            CreateMap<AdsEntity, AdsModel>();
            CreateMap<AdsModel, AdsEntity>();

            CreateMap<LockerBookingEntity, LockerMobileBookingModel>();
            CreateMap<LockerMobileBookingModel, LockerBookingEntity>();

            CreateMap<LockerBookingEntity, LockerManualBookingModel>();
            CreateMap<LockerManualBookingModel, LockerBookingEntity>();

            CreateMap<PaymentMethodEntity, PaymentMethodModel>();
            CreateMap<PaymentMethodModel, PaymentMethodEntity>();

            CreateMap<PaymentInfoEntity, PaymentInfoModel>();
            CreateMap<PaymentInfoModel, PaymentInfoEntity>();

            CreateMap<ApplicationSettingEntity, ApplicationSettingModel>();
            CreateMap<ApplicationSettingModel, ApplicationSettingEntity>();

            CreateMap<MessageEntity, MessageModel>();
            CreateMap<MessageModel, MessageEntity>();

            CreateMap<ApplicationMessageEntity, ApplicationMessageModel>();
            CreateMap<ApplicationMessageModel, ApplicationMessageEntity>();


            CreateMap<MessageEntity, SendEmailModel>();
            CreateMap<SendEmailModel, MessageEntity>();
            CreateMap<PaymentTransactionEntity, PaymentTransactionModel>();
            CreateMap<PaymentTransactionModel, PaymentTransactionEntity>();
            CreateMap<CompanyEntity, CompanyModel>();
            CreateMap<CompanyModel, CompanyEntity>();

            CreateMap<CompanyEntity, CompanyViewModel>();
            CreateMap<CompanyEntity, CompanyLocationModel>();

            CreateMap<ParentCompanyBaseModel, ParentCompanyEntity>();
            CreateMap<ParentCompanyEntity, ParentCompanyBaseModel>();

            CreateMap<ParentCompanyViewModel, ParentCompanyEntity>();
            CreateMap<ParentCompanyEntity, ParentCompanyViewModel>();


            CreateMap<CabinetTypeViewModel, CabinetTypeEntity>();
            CreateMap<CabinetTypeEntity, CabinetTypeViewModel>();

            CreateMap<LockerZoneModel, LockerZoneEntity>();
            CreateMap<LockerZoneEntity, LockerZoneModel>();

            CreateMap<ActivatonCompanyUserModel, CompanyUserEntity>();
            CreateMap<PostCompanyUserModel, CompanyUserEntity>();
            CreateMap<PostUpdateCompanyUserModel, CompanyUserEntity>();
            CreateMap<PostUpdateCompanyUserPasswordModel, CompanyUserEntity>();
            CreateMap<CompanyUserEntity, CompanyUserViewModel>();

            CreateMap<RoleModel, RoleEntity>();
            CreateMap<RoleEntity, RoleModel>();
            CreateMap<RoleViewModel, RoleEntity>();
            CreateMap<RoleEntity, RoleViewModel>();



            CreateMap<UserRoleModel, UserRoleEntity>();
            CreateMap<UserRoleEntity, UserRoleModel>();
            CreateMap<UserRoleViewModel, UserRoleEntity>();
            CreateMap<UserRoleEntity, UserRoleViewModel>();
            CreateMap<MockUserRole, UserRoleEntity>();
            CreateMap<UserRoleEntity, MockUserRole>();

            CreateMap<PermissionModel, PermissionEntity>();
            CreateMap<PermissionEntity, PermissionModel>();
            CreateMap<PermissionViewModel, PermissionEntity>();
            CreateMap<PermissionEntity, PermissionViewModel>();

            CreateMap<RolePermissionModel, RolePermissionEntity>();
            CreateMap<RolePermissionEntity, RolePermissionModel>();
            CreateMap<RolePermissionViewModel, RolePermissionEntity>();
            CreateMap<RolePermissionEntity, RolePermissionViewModel>();
            CreateMap<MockRolePermissionModel, RolePermissionEntity>();
            CreateMap<RolePermissionEntity, MockRolePermissionModel>();
            CreateMap<RolePermissionEntity, RolePermissionDetailModel>();


            CreateMap<FeedbackModel, FeedbackEntity>();
            CreateMap<FeedbackEntity, FeedbackModel>();
            CreateMap<FeedbackViewModel, FeedbackEntity>();
            CreateMap<FeedbackEntity, FeedbackViewModel>();

            CreateMap<FranchiseFeedbackQuestionModel, FranchiseFeedbackQuestionEntity>();
            CreateMap<FranchiseFeedbackQuestionEntity, FranchiseFeedbackQuestionModel>();
            CreateMap<FranchiseFeedbackQuestionViewModel, FranchiseFeedbackQuestionEntity>();
            CreateMap<FranchiseFeedbackQuestionEntity, FranchiseFeedbackQuestionViewModel>();

            CreateMap<FranchiseFeedbackAnswerModel, FranchiseFeedbackAnswerEntity>();
            CreateMap<FranchiseFeedbackAnswerEntity, FranchiseFeedbackAnswerModel>();
            CreateMap<FranchiseFeedbackAnswerViewModel, FranchiseFeedbackAnswerEntity>();
            CreateMap<FranchiseFeedbackAnswerEntity, FranchiseFeedbackAnswerViewModel>();

            CreateMap<EmailModel, EmailEntity>();
            CreateMap<EmailEntity, EmailModel>();
            CreateMap<EmailViewModel, EmailEntity>();
            CreateMap<EmailEntity, EmailViewModel>();
            CreateMap<LockerBookingEntity, LockerBookingViewModel>().ReverseMap();

            CreateMap<CompanyLocationEntity, CompanyLocationModel>().ReverseMap();

            CreateMap<LockerDetailBooking, LockerDetailEntity>().ReverseMap();
            CreateMap<LockerBookingEntity, PaymentTransactionModel>().ReverseMap();
            CreateMap<LockerBookingViewModel, LockerBookingPaymentDetail>().ReverseMap();

            CreateMap<LockerBookingPaymentDetail, PaymentTransactionModel>().ReverseMap();

        }
    }
}
