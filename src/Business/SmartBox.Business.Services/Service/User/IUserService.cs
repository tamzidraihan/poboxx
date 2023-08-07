using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Services.Service.Base;

namespace SmartBox.Business.Services.Service.User
{
    public interface IUserService : IBaseMessageService<IUserService>
    {
        Task<PostRegistrationModel> SetUser(UserFormModel userModel);
        Task<ResponseValidityModel> SetUser(AdminUserFormModel adminUserForm);
        Task<ResponseValidityModel> UpdateUser(UserUpdateModel userModel);
        Task<ResponseValidityModel> UpdateUserMPIN(UserMPINModel userMPINModel);
        Task<PostRegistrationModel> UpdateUserOTP(UserModel userModel, bool isEmailOTPOnly);
        Task<ResponseValidityModel> ForgetPassword(ForgetPasswordModel forgetPasswordModel);
        Task<UserModel> GetUser(string phoneNumber);
        Task<List<UserModel>> GetAppUsers();
        Task<UserModel> GetUserByUserKeyId(string userKeyId);
        Task<List<AdminUserModel>> GetAdminUsers(string userName = null, string email = null);
        Task<AdminUserModel> GetAdminUser(string username, bool includeRoles = true);
        Task<ResponseValidityModel> ActivateOTP(string phoneNumber, string OTP);
        Task<ResponseValidityModel> ValidateNewMobileOTP(string userKeyId, string OTP);
        Task<ResponseValidityModel> SaveCharges(ChargesModel param);
        Task<List<ChargesViewModel>> GetChargesByUser(string userKeyI, string paymentRefNo = null);
        Task<ResponseValidityModel> DeleteCharges(int id);
        Task<ResponseValidityModel> SaveUserSubscription(UserSubscriptionModel param, string userkeyId = null);
        Task<List<UserSubscriptionViewModel>> GetSubscriptionsByUser(string userKeyId);
        Task<ResponseValidityModel> DeleteUserSubscription(int id);
        Task ExpiredLockerBookingsNotifications();
        Task<ResponseValidityModel> DeleteAdminUser(int id);
        Task<ResponseValidityModel> DeleteAppUser(string userKeyId);

        Task<ResponseValidityModel> CreateUserFavouriteCabinetLocations(string userKeyId, int cabinetLocartionId);
        Task<ResponseValidityModel> DeleteUserFavouriteCabinetLocations(int Id);
        Task<List<CabinetLocationViewModel>> GetUserFavoritesCabinetLocations(string userKeyId);


        #region User Subscription Billing
        Task<ResponseValidityModel> SaveUserSubscriptionBilling(UserSubscriptionBillingModel param);
        Task<List<UserSubscriptionBillingModel>> GetUserSubscriptionBillingBySubscriptionId(int userSubscriptionId);
        Task<ResponseValidityModel> DeleteUserSubscriptionBilling(int id);
        #endregion
        #region User Token
        Task<ResponseValidityModel> SaveUserToken(UserTokenModel param, string userKeyId, string deviceDescription);
        Task<List<UserTokenModel>> GetUserTokenByUserId(string userKeyId, string deviceDescription = "", DeviceType? deviceType = null);
        Task UserLastHourSubscriptionNotification(int id);
        Task UserLastWeekSubscriptionNotification(int id);
        Task UserExpiredBookingNotificaiton(int id);
        #endregion
    }
}
