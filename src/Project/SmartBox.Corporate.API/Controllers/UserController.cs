using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Services.Service.User;
using Microsoft.AspNetCore.Authorization;
using static SmartBox.Business.Shared.GlobalConstants;
using static SmartBox.Business.Shared.GlobalMessageView;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Business.Services.Service.Locker;
using Org.BouncyCastle.Asn1.Ocsp;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using SmartBox.Business.Services.Service.Notification;
using SmartBox.Business.Core.Models.Booking;
using static SmartBox.Business.Shared.GlobalEnums;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Models.Cabinet;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILockerService lockerService;
        public UserController(IUserService userService, ILockerService lockerService)
        {
            _userService = userService;
            this.lockerService = lockerService;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="userFormModel">Request's payload</param>
        /// <returns> returns PostRegistrationModel with token when success</returns>
        /// <remarks> all fields are required</remarks>
        /// <response code="200">User successfully registered</response>
        [HttpPost("RegisterUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<PostRegistrationModel>> RegisterUser([BindRequired, FromBody] Business.Core.Models.User.UserFormModel userFormModel)
        {
            var model = await _userService.SetUser(userFormModel);
            if (model.ValidityModel.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model.ValidityModel);
            }
        }
        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="userUpdateModel">Request's payload</param>
        /// <returns> returns ResponseValidityModel with token when success</returns>

        /// <response code="200">User successfully updated</response>
        [HttpPost("UpdateUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> UpdateUser([BindRequired, FromBody] Business.Core.Models.User.UserUpdateModel userUpdateModel)
        {
            var model = await _userService.UpdateUser(userUpdateModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        /// <summary>
        /// Update User MPIN
        /// </summary>
        /// <param name="userMPINModel">Request's payload</param>
        /// <returns> returns ResponseValidityModel with token when success</returns>
        /// <remarks> all fields are required</remarks>
        /// <response code="200">User MPIN successfully updated</response>
        [HttpPost("UpdateMPIN")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> UpdateMPIN([BindRequired, FromBody] Business.Core.Models.User.UserMPINModel userMPINModel)
        {
            var model = await _userService.UpdateUserMPIN(userMPINModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        /// <summary>
        /// Register Adminstrator
        /// </summary>
        /// <param name="adminUserForm">Request's payload</param>
        /// <returns> returns PostRegistrationModel with token when success</returns>
        /// <remarks> all fields are required.</remarks>
        /// <response code="200">User successfully registered</response>
        [HttpPost("RegisterAdminUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<PostRegistrationModel>> RegisterAdminUser([BindRequired, FromBody] AdminUserFormModel adminUserForm)
        {
            var model = await _userService.SetUser(adminUserForm);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Update Adminstrator
        /// </summary>
        /// <param name="adminUserForm">Request's payload</param>
        /// <returns> returns PostRegistrationModel with token when success</returns>
        /// <remarks> AdminUserId should not be 0 or empty</remarks>
        /// <response code="200">User successfully updated</response>
        [HttpPut("UpdateAdminUser")]
      
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> UpdateAdminUser([BindRequired, FromBody] AdminUserFormModel adminUserForm)
        {
            var model = await _userService.SetUser(adminUserForm);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Delete Admin user. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [Authorize]
        [HttpDelete("DeleteAdminUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteAdminUser([FromQuery] int id)
        {
            var model = await _userService.DeleteAdminUser(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        /// <summary>
        /// OTP Generation for new user registration
        /// </summary>
        /// <param name="phoneNumber">the phoneNumber of the registered user</param>
        /// <returns> returns PostRegistrationModel with updated OTP for user upon activation</returns>
        /// <response code="200">OTP Succesfully generate</response>
        [HttpPut("GenerateRegistrationOTP")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<PostRegistrationModel>> GenerateRegistrationOTP([BindRequired] string phoneNumber)
        {
            var ret = await _userService.GetUser(phoneNumber);
            if (ret.ValidityModel.MessageReturnNumber != ApplicationMessageNumber.ErrorMessage.NotExistingUser)
            {
                var model = new UserModel
                {
                    UserKeyId = ret.UserKeyId,
                    PhoneNumber = ret.PhoneNumber,
                    Email = ret.Email
                };

                var response = await _userService.UpdateUserOTP(model, false);
                if (response.ValidityModel.MessageReturnNumber > 0)
                    return Ok(response);
                else
                {
                    return Conflict(response);
                }
            }
            else
            {
                return Conflict(ret.ValidityModel);
            }
        }

        /// <summary>
        /// OTP Generation for update phone number
        /// </summary>
        /// 
        /// <param name="userOTPModel">the phoneNumber of the registered user</param>
        /// <returns> returns PostRegistrationModel with updated OTP for user upon activation</returns>
        /// <response code="200">OTP Succesfully generate</response>
        [HttpPut("GenerateUpdateMobileOTP")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<PostRegistrationModel>> GenerateUpdateMobileOTP([BindRequired, FromBody] UserOTPModel userOTPModel)
        {
            var ret = await _userService.GetUserByUserKeyId(userOTPModel.UserKeyId);
            if (ret.ValidityModel.MessageReturnNumber != ApplicationMessageNumber.ErrorMessage.NotExistingUser)
            {
                
                var model = new UserModel
                {
                    UserKeyId = userOTPModel.UserKeyId,
                    PhoneNumber = userOTPModel.PhoneNumber,
                    Email = userOTPModel.NewEmail != null ? userOTPModel.NewEmail : ret.Email
                };

                var response = await _userService.UpdateUserOTP(model, userOTPModel.IsEmailOTPOnly);

                if (response.ValidityModel.MessageReturnNumber > 0)
                    return Ok(response);
                else
                {
                    return Conflict(response);
                }
            }
            else
            {
                return Conflict(ret.ValidityModel);
            }
        }



        /// <summary>
        /// Validate the PIN for updating mobile number
        /// <br/>
        /// </summary>
        /// <param name="OTP">OTP reveived from the new number</param>
        /// <param name="userKeyId">UserKeyId of the user being udpated</param>
        /// <returns> returns if the PIN is valid</returns>

        /// <response code="200">OTP is valid</response>
        [HttpGet("ValidateNewMobileOTP")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> ValidateNewMobileOTP(string userKeyId, string OTP)
        {
            var ret = await _userService.ValidateNewMobileOTP(userKeyId, OTP);
            if (ret.MessageReturnNumber == ApplicationMessageNumber.InformationMessage.ValidOTP)
                return Ok(ret);
            else

                return Conflict(ret);
        }

        /// <summary>
        /// Get the log-in user information, no parameter needed, user must be log-in
        /// </summary>
        /// <returns> returns User model with user information</returns>
        /// <response code="200">User model</response>
        [Authorize]
        [HttpGet("GetLogInUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<UserModel>> GetLogInUser()
        {
            var response = await _userService.GetUserByUserKeyId(User.Identity.Name);
            if (response.ValidityModel.MessageReturnNumber > 0)
                return Ok(response);
            else
            {
                return Conflict(response);
            }
        }
        [HttpPost("ForgetPassword")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> ForgetPassword(ForgetPasswordModel model)
        {
            var response = await _userService.ForgetPassword(model);
            if (response.MessageReturnNumber > 0)
                return Ok(response);
            else
            {
                return Conflict(response);
            }
        }

        /// <summary>
        /// Get the admin users , can be filter by user name or email from params
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns>returns Admin User model with admin user information</returns>
        /// <response code="200">Admin User model</response>
        [Authorize]
        [HttpGet("GetAdminUsers")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<AdminUserModel>> GetAdminUsers(string userName = null, string email = null)
        {
            var response = await _userService.GetAdminUsers(userName, email);
            return Ok(response);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>returns User model with user information</returns>
        /// <response code="200">User model</response>
        [Authorize]
        [HttpGet("GetAppUsers")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<UserModel>> GetAppUsers()
        {
            var response = await _userService.GetAppUsers();
            return Ok(response);
        }
        /// <summary>
        /// Delete App user. UserId is required
        /// <br/>
        /// </summary>
        /// <param name="userKeyId">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
   
        [Authorize]
        [HttpDelete("DeleteAppUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteAppUser([FromQuery] string userKeyId)
        {

            var model = await _userService.DeleteAppUser(userKeyId);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);

        }
        /// <summary>
        /// Get the admin log-in user information, no parameter needed, user must be log-in
        /// </summary>
        /// <returns> returns User model with user information</returns>
        /// <response code="200">User model</response>
        [Authorize]
        [HttpGet("GetAdminLogInUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]

        public async Task<ActionResult<UserModel>> GetAdminLogInUser()
        {
            var response = await _userService.GetAdminUser(User.Identity.Name);
            return Ok(response);
        }

        #region Charges
        /// <summary>
        /// Get a list of charges by the user
        /// </summary>
        /// <returns> returns list charges by the user</returns>
        /// <response code="200">ChargesViewModel list</response>
        [Authorize]
        [HttpGet("GetChargesByUserKeyId")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<ChargesViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<ChargesViewModel>>> GetChargesByUser(string userKeyId)
        {
            var model = await _userService.GetChargesByUser(userKeyId);
            return Ok(model);
        }
        /// <summary>
        /// Add or update a charges. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="chargesModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [Authorize]
        [HttpPost("SaveCharges")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveCharges([FromBody] ChargesModel chargesModel)
        {
            var model = await _userService.SaveCharges(chargesModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        #endregion
        #region User Subscription
        /// <summary>
        /// Get a list of user Subscriptions 
        /// </summary>
        /// <returns> returns list user Subscriptions </returns>
        /// <response code="200">UserSubscriptionViewModel list</response>
        [Authorize]
        [HttpGet("GetSubscriptionsByUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<UserSubscriptionViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<UserSubscriptionViewModel>>> GetSubscriptionsByUser(string userKeyId)
        {
            var model = await _userService.GetSubscriptionsByUser(userKeyId);
            return Ok(model);
        }

        /// <summary>
        /// Add or update a user Subscription. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="userSubscriptionModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [Authorize]
        [HttpPost("SaveUserSubscription")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveUserSubscription([FromBody] UserSubscriptionModel userSubscriptionModel)
        {
            var model = await _userService.SaveUserSubscription(userSubscriptionModel, User.Identity.Name);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        ///// <summary>
        ///// Delete user Subscription. Id is required
        ///// <br/>
        ///// </summary>
        ///// <param name="id">Request's payload</param>
        ///// <returns> returns if validity model if successfully delete</returns>
        ///// <response code="200">ResponseValidityModel either success or failed</response>
        //[Authorize]
        //[HttpDelete("DeleteUserSubscription")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        //[ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        //public async Task<ActionResult<ResponseValidityModel>> DeleteUserSubscription([FromQuery] int id)
        //{
        //    var model = await _userService.DeleteUserSubscription(id);
        //    if (model.MessageReturnNumber > 0)
        //        return Ok(model);
        //    else
        //        return Conflict(model);
        //}
        #endregion
        #region User Subscription Billing
        /// <summary>
        /// Get a list of user Subscription billings 
        /// </summary>
        /// <returns> returns list user Subscription billings </returns>
        /// <response code="200">UserSubscriptionBillingModel list</response>
        [Authorize]
        [HttpGet("GetSubscriptionBillingsBySubscriptionId")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<UserSubscriptionModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<UserSubscriptionBillingModel>>> GetUserSubscriptionBillingBySubscriptionId(int userSubscriptionId)
        {
            var model = await _userService.GetUserSubscriptionBillingBySubscriptionId(userSubscriptionId);
            return Ok(model);
        }

        /// <summary>
        /// Add or update a user Subscription billing. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="userSubscriptionBillingModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [Authorize]
        [HttpPost("SaveUserSubscriptionBilling")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveUserSubscriptionBilling([FromBody] UserSubscriptionBillingModel userSubscriptionBillingModel)
        {
            var model = await _userService.SaveUserSubscriptionBilling(userSubscriptionBillingModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Delete user Subscription Billing. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [Authorize]
        [HttpDelete("DeleteUserSubscriptionBilling")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteUserSubscriptionBilling([FromQuery] int id)
        {
            var model = await _userService.DeleteUserSubscriptionBilling(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        #endregion
        #region User Token
        /// <summary>
        /// Get a list of user tokens 
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns> returns user token list </returns>
        /// <response code="200">UserTokenModel list</response>
        [Authorize]
        [HttpGet("GetUserToken")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<UserTokenModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<UserTokenModel>>> GetUserTokens(DeviceType? deviceType = null)
        {
            var model = await _userService.GetUserTokenByUserId(User.Identity.Name, "", deviceType);
            return Ok(model);
        }

        /// <summary>
        /// Add or update a user token. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="userTokenModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [Authorize]
        [HttpPost("SaveUserToken")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveUserToken([FromBody] UserTokenModel userTokenModel)
        {
            var deviceDescription = HttpContext.Request.Headers["User-Agent"];
            var model = await _userService.SaveUserToken(userTokenModel, User.Identity.Name, deviceDescription);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        #endregion
        #region User Locker Details
        /// <summary>
        /// Get analytics for the User
        /// </summary>
        /// <returns> returns user Locker Booking Report </returns>
        /// <response code="200">UserLockerBookingReportModel</response>
        [Authorize]
        [HttpGet("GetBookingTransactionsReport")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(UserLockerBookingReportModel), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<UserLockerBookingReportModel>> GetBookingTransactionsReport()
        {
            var model = await lockerService.GetBookingTransactionsReport(User.Identity.Name);
            return Ok(model);
        }

        [Authorize]
        [HttpGet("GetUserBookingTransactions")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(BookingTransactionsViewModel), (int)HttpStatusCode.Accepted)]

        public async Task<ActionResult<BookingTransactionsViewModel>> GetBookingTransactions(string userKeyId, DateTime? startDate, DateTime?
                                                                                endDate, int? companyId = null, int? currentPage = null, int? pageSize = null, BookingTransactionStatus? bookingStatus = null)
        {
            var model = await lockerService.GetUserBookingTransactions(userKeyId, startDate, endDate, companyId,
                currentPage, pageSize, bookingStatus);
            return Ok(model);
        }


        [Authorize]
        [HttpPost("CreateUserFavouriteCabinetLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]

        public async Task<ActionResult<ResponseValidityModel>> CreateUserFavouriteCabinetLocations(UserFavouriteLocationModel param)
        {
            var model = await _userService.CreateUserFavouriteCabinetLocations(param.UserKeyId, param.CabinetLocationId);
            return Ok(model);
        }

       [Authorize]
        [HttpDelete("DeleteUserFavouriteCabinetLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]

        public async Task<ActionResult<ResponseValidityModel>> DeleteUserFavouriteCabinetLocations([FromQuery]  int Id)
        {
            var model = await _userService.DeleteUserFavouriteCabinetLocations(Id);
            return Ok(model);
        }

       [Authorize]
        [HttpGet("GetUserFavoritesCabinetLocations")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(CabinetLocationViewModel), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<CabinetLocationViewModel>> GetUserFavoritesCabinetLocations(string userKeyId)
        {
            var model = await _userService.GetUserFavoritesCabinetLocations(userKeyId);
            return Ok(model);
        }

        #endregion
    }
}
