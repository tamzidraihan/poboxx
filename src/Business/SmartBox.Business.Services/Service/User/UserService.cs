using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Entities.Notification;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Core.Entities.UserRole;
using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Email;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Core.Models.UserRole;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.HTTPService;
using SmartBox.Business.Services.Service.LogIn;
using SmartBox.Business.Services.Service.Notification;
using SmartBox.Business.Services.Service.Notification.PushNotification;
using SmartBox.Business.Services.Service.OTP;
using SmartBox.Business.Services.Service.UserRole;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.ApplicationSettings;
using SmartBox.Infrastructure.Data.Repository.AppMessage;
using SmartBox.Infrastructure.Data.Repository.Locker;
using SmartBox.Infrastructure.Data.Repository.User;
using SmartBox.Infrastructure.Data.Repository.UserRole;
using static Slapper.AutoMapper;
using static SmartBox.Business.Shared.GlobalConstants;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Services.Service.User
{
    public class UserService : BaseMessageService<UserService>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IApplicationSettingRepository _appSettingRepository;
        private readonly INotificationService _notificationService;
        private readonly IChargesRepository chargesRepository;
        private readonly IOTPService _iOTPService;
        private readonly ILogger<LogInService> _logger;
        private readonly IUserSubscriptionRepository userSubscriptionRepository;
        private readonly IUserSubscriptionBillingRepository userSubscriptionBillingRepository;
        private readonly IUserTokenRepository userTokenRepository;
        private readonly GlobalConfigurations globalConfig;
        private readonly HangfireConfig hangfireConfig;
        private readonly IHttpService _httpService;
        private readonly ILockerRepository lockerRepository;
        private readonly IUserRoleService userRoleService;
        public UserService(IAppMessageService appMessageService, IMapper mapper,
                          IUserRepository userRepository,
                          IUserRoleService userRoleService,
                          ILockerRepository lockerRepository,
                          IOptions<GlobalConfigurations> options,
                           IOptions<HangfireConfig> hangfireConfig,
                          IUserSubscriptionBillingRepository userSubscriptionBillingRepository,
                          IChargesRepository chargesRepository,
                          IUserTokenRepository userTokenRepository,
                          IUserSubscriptionRepository userSubscriptionRepository,
                          IApplicationSettingRepository appSettingRepository,
                          INotificationService notificationService,
                          IUserRoleRepository userRoleRepository,
                          IHttpService httpService,
                          IOTPService iOTPService, ILogger<LogInService> logger) : base(appMessageService, mapper)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _appSettingRepository = appSettingRepository;
            _notificationService = notificationService;
            _iOTPService = iOTPService;
            this.hangfireConfig = hangfireConfig.Value;
            this.globalConfig = options.Value;
            this.chargesRepository = chargesRepository;
            this.userTokenRepository = userTokenRepository;
            _logger = logger;
            this.lockerRepository = lockerRepository;
            this.userSubscriptionBillingRepository = userSubscriptionBillingRepository;
            this.userSubscriptionRepository = userSubscriptionRepository;
            _httpService = httpService;
            this.userRoleService = userRoleService;
        }

        async Task<bool> IsExisting(string emailAddress)
        {
            var entity = await _userRepository.GetUserFromPhoneNo(emailAddress);
            return entity != null;
        }

        ResponseValidityModel ValidateIsExistingUser(string emailAddress)
        {
            var model = new ResponseValidityModel();

            if (IsExisting(emailAddress).Result)
            {
                model = this.AppMessageService.SetMessage(-1).MappedResponseValidityModel();
            }

            return model;
        }
        PostRegistrationModel ValidateSaveModel(UserFormModel userModel)
        {
            var validation = ValidateIsExistingUser(userModel.PhoneNumber);
            var model = new PostRegistrationModel
            {
                ValidityModel = validation
            };

            if (model.ValidityModel.MessageReturnNumber < 0)
            {
                return model;
            }

            if (!userModel.FirstName.HasText())
            {
                model.ValidityModel.MessagesList.Add(GlobalMessageView.UserFormModel.FirstName);
            }

            if (!userModel.LastName.HasText())
            {
                model.ValidityModel.MessagesList.Add(GlobalMessageView.UserFormModel.LastName);
            }

            if (!userModel.Email.HasText())
            {
                model.ValidityModel.MessagesList.Add(GlobalMessageView.UserFormModel.Email);
            }

            if (!userModel.PhoneNumber.HasText())
            {
                model.ValidityModel.MessagesList.Add(GlobalMessageView.UserFormModel.PhoneNumber);
            }

            if (model.ValidityModel.MessagesList.Count > 0)
            {
                model.ValidityModel = this.AppMessageService.SetMessage(-601).MappedResponseValidityModel();
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(GlobalConstants.MessageParameters.Field, model.ValidityModel.MessagesList.Count > 1 ? Shared.SharedServices.SetVerbMessage(true) : Shared.SharedServices.SetVerbMessage(false));
            }

            return model;
        }

        public async Task<PostRegistrationModel> SetUser(UserFormModel userModel)
        {
            var model = ValidateSaveModel(userModel);

            if (model.ValidityModel.MessageReturnNumber == 0)
            {
                var emailExists = await _userRepository.GetUserFromEmail(userModel.Email);
                if (emailExists != null)
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.EmailAlreadyExists).MappedResponseValidityModel();
                    return model;
                }
                var phoneNoEists = await _userRepository.GetUserFromPhoneNo(userModel.PhoneNumber);
                if (phoneNoEists != null)
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.PhoneNoAlreadyExists).MappedResponseValidityModel();
                    return model;
                }
                userModel.Password = Shared.SharedServices.HashPassword(userModel.Password);
                var entity = Mapper.Map<UserFormModel, UserEntity>(userModel);
                var appSetting = await _appSettingRepository.GetApplicationSetting();

                entity.OTP = await _iOTPService.GenerateOTP();
                entity.OTPExpirationDate = _iOTPService.OTPExpiration();

                var id = await _userRepository.GetLastIdentity();
                entity.UserKeyId = Shared.SharedServices.GenerateLastUserKeyId(id + 1);

                var ret = await _userRepository.SetUserEntity(entity);

                if (ret > 0)
                {
                    model.OTP = entity.OTP;
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.RecordAdded).MappedResponseValidityModel();
                    var msg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.SMSOTP);

                    var refcode = string.Concat(DateTime.Now.Year.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Millisecond.ToString(), entity.PhoneNumber);

                    msg.Message = msg.Message.Replace(MessageParameters.Pin, entity.OTP);
                    msg.Message = msg.Message.Replace(MessageParameters.Validity, appSetting.OTPMinutes.ToString());

                    var isSent = await _httpService.SendSMS(entity.PhoneNumber, msg.Message, refcode.ToUpper());
                    if (isSent)
                        _logger.LogInformation("Success sending of SMS code for user registration with email " + userModel.Email);
                    else
                        _logger.LogError("Failed sending of SMS code for user registration with email " + userModel.Email);

                    isSent = await _notificationService.SendSmtpEmailAsync(new EmailModel
                    {
                        Subject = $"{globalConfig.AppName} -  One Time Password",
                        Message = msg.Message,
                        To = userModel.Email,
                        Type = MessageType.HtmlContent
                    });

                    if (isSent)
                        _logger.LogInformation("Success sending of email for user registration with email " + userModel.Email);
                    else
                        _logger.LogError("Failed sending of email for user registration with email " + userModel.Email);

                }
                else
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
                }
            }

            return model;
        }

        public async Task<ResponseValidityModel> UpdateUserMPIN(UserMPINModel userMPINModel)
        {

            UserUpdateModel userModel = new UserUpdateModel();

            userModel.UserMPIN = userMPINModel.MPIN;

            var model = new ResponseValidityModel();

            var entity = await _userRepository.GetUserByUserKeyId(userMPINModel.UserKeyId);

            if (entity == null)
            {
                return userModel.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingUser).MappedResponseValidityModel();
            }

            entity.MPIN = userMPINModel.MPIN;

            var ret = await _userRepository.UpdateUserMPIN(entity);

            if (ret > 0)
            {
                model = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.RecordUpdated).MappedResponseValidityModel();
            }
            else
            {
                return AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            }

            return model;
        }


        public async Task<ResponseValidityModel> UpdateUser(UserUpdateModel userModel)
        {
            UserFormModel model = new UserFormModel();
            var dbModel = await _userRepository.GetUserByUserKeyId(userModel.UserKeyId);

            if (dbModel == null)
            {
                model = new UserModel();
                return model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingUser).MappedResponseValidityModel();
            }



            userModel.Password = Shared.SharedServices.HashPassword(userModel.Password);
            var entity = Mapper.Map<UserUpdateModel, UserEntity>(userModel);

            entity.UserId = dbModel.UserId;
            entity.UserKeyId = dbModel.UserKeyId;

            var ret = await _userRepository.SetUserEntity(entity);

            if (ret > 0)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.RecordUpdated).MappedResponseValidityModel();
            }
            else
            {
                return AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            }

            return model.ValidityModel;

        }

        public async Task<List<UserModel>> GetAppUsers()
        {
            var entities = await _userRepository.GetAppUsers();
            var model = Mapper.Map<List<UserModel>>(entities);

            return model;
        }

        public async Task<UserModel> GetUser(string phoneNumber)
        {
            var dbModel = await _userRepository.GetUserFromPhoneNo(phoneNumber);
            var model = Mapper.Map<UserEntity, UserModel>(dbModel);

            if (dbModel != null)
            {
                if (dbModel.IsActivated && !dbModel.IsDeleted)
                {
                    //exisiting active user 
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.RecordExists).MappedResponseValidityModel();
                }
                else if (!dbModel.IsActivated && !dbModel.IsDeleted)
                {
                    //exisiting unactivated user 
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.OTPUnactivated).MappedResponseValidityModel();
                }
                else
                {
                    //inactive user
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InActiveuser).MappedResponseValidityModel();
                }
            }
            else
            {
                //not existing
                model = new UserModel();
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingUser).MappedResponseValidityModel();
            }

            return model;
        }

        public async Task<UserModel> GetUserByUserKeyId(string userKeyId)
        {
            var dbModel = await _userRepository.GetUserByUserKeyId(userKeyId);
            var model = Mapper.Map<UserEntity, UserModel>(dbModel);

            if (dbModel != null)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.RecordExists).MappedResponseValidityModel();
            }
            else
            {
                //not existing
                model = new UserModel();
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingUser).MappedResponseValidityModel();
            }

            return model;
        }
        public async Task<ResponseValidityModel> ForgetPassword(ForgetPasswordModel forgetPasswordModel)
        {
            var model = ValidateForgetPassword(forgetPasswordModel);

            if (model.MessageReturnNumber == 0)
            {
                UserEntity user = null;
                bool isSent = false;
                if (!string.IsNullOrEmpty(forgetPasswordModel.Email))
                {
                    user = await _userRepository.GetUserFromEmail(forgetPasswordModel.Email);
                    if (user != null)
                    {
                        var newPassword = SharedServices.GenerateRandomPassword(10);
                        var newHashPassword = SharedServices.HashPassword(newPassword);
                        var ret = await _userRepository.UpdateUserPassword(user.UserId, newHashPassword);
                        if (ret > 0)
                        {
                            var email = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.ForgetPasswordEmail);
                            email.Message = email.Message.Replace(MessageParameters.UserName, user.FirstName);
                            email.Message = email.Message.Replace(MessageParameters.Password, newPassword);

                            isSent = await _notificationService.SendSmtpEmailAsync(new EmailModel
                            {
                                Subject = "New password has been generated",
                                Message = email.Message,
                                To = user.Email,
                                Type = MessageType.HtmlContent
                            });

                            if (isSent)
                                _logger.LogInformation("Success sending of email for user forget password with email " + user.Email);
                            else
                                _logger.LogError("Failed sending of email for user forget password with email " + user.Email);
                        }
                    }
                }

                if (user == null && !string.IsNullOrEmpty(forgetPasswordModel.PhoneNumber))
                {
                    user = await _userRepository.GetUserFromPhoneNo(forgetPasswordModel.PhoneNumber);
                    if (user != null)
                    {
                        var newPassword = SharedServices.GenerateRandomPassword(10);
                        var newHashPassword = SharedServices.HashPassword(newPassword);
                        var ret = await _userRepository.UpdateUserPassword(user.UserId, newHashPassword);
                        if (ret > 0)
                        {
                            var msg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.ForgetPasswordSms);
                            msg.Message = msg.Message.Replace(MessageParameters.Password, newPassword);
                            var refCode = GlobalExtension.GenerateNewGuidRefCode();
                            isSent = await _httpService.SendSMS(user.PhoneNumber, msg.Message, refCode.ToUpper());
                            if (isSent)
                                _logger.LogInformation("Success sending of SMS password code for user registration with phone " + user.PhoneNumber);
                            else
                                _logger.LogError("Failed sending of SMS password code for user registration with phone " + user.PhoneNumber);
                        }
                    }
                }
                if (isSent)
                    model = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.PasswordDelivered).MappedResponseValidityModel();
                else
                    model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            }
            else
            {
                model = AppMessageService.SetMessage(model.MessageReturnNumber).MappedResponseValidityModel();
            }
            return model;
        }

        ResponseValidityModel ValidateForgetPassword(ForgetPasswordModel param)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(param.Email)
                && string.IsNullOrEmpty(param.PhoneNumber))
            {
                model.MessageReturnNumber = ApplicationMessageNumber.InformationMessage.EmailOrPhoneRequired;
            }

            var isExist = GetAppUsers().Result.Where(e => e.Email == param.Email || e.PhoneNumber == param.PhoneNumber).FirstOrDefault();

            if (isExist == null)
                model.MessageReturnNumber = ApplicationMessageNumber.ErrorMessage.NotExistingUser;

            return model;
        }

        public async Task<ResponseValidityModel> ValidateNewMobileOTP(string userKeyId, string OTP)
        {
            var model = await GetUserByUserKeyId(userKeyId);
            var appsettings = await _appSettingRepository.GetApplicationSetting();

            if (model.ValidityModel.MessageReturnNumber == ApplicationMessageNumber.InformationMessage.RecordExists)
            {
                var model2 = await GetUser(model.PhoneNumber);
                TimeSpan timeSpan = model2.OTPExpirationDate.Subtract(DateTime.Now);

                if ((DateTime.Now - model2.OTPExpirationDate).TotalMinutes <= appsettings.OTPMinutes)
                {
                    if (model2.OTP == OTP)
                    {

                        model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.ValidOTP).MappedResponseValidityModel();

                    }
                    else
                    {
                        model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidOTP).MappedResponseValidityModel();
                    }
                }
                else
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.OTPExpired).MappedResponseValidityModel();
                }

            }
            else if (model.ValidityModel.MessageReturnNumber == ApplicationMessageNumber.ErrorMessage.NotExistingUser)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingUser).MappedResponseValidityModel();
            }

            return model.ValidityModel;
        }

        public async Task<ResponseValidityModel> ActivateOTP(string phoneNumber, string OTP)
        {
            var model = await GetUser(phoneNumber);
            var appsettings = await _appSettingRepository.GetApplicationSetting();

            if (model.ValidityModel.MessageReturnNumber == ApplicationMessageNumber.ErrorMessage.OTPUnactivated)
            {
                TimeSpan timeSpan = model.OTPExpirationDate.Subtract(DateTime.Now);

                if ((DateTime.Now - model.OTPExpirationDate).TotalMinutes <= appsettings.OTPMinutes)
                {
                    if (model.OTP == OTP)
                    {
                        var ret = await _userRepository.ActivateUser(phoneNumber);
                        if (ret > 0)
                            model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.AccountActivated).MappedResponseValidityModel();
                        else
                            model.ValidityModel = AppMessageService.SetMessage((short)ret).MappedResponseValidityModel();
                    }
                    else
                    {
                        model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidOTP).MappedResponseValidityModel();
                    }
                }
                else
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.OTPExpired).MappedResponseValidityModel();
                }

            }
            else if (model.ValidityModel.MessageReturnNumber == ApplicationMessageNumber.InformationMessage.RecordExists)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.OTPActivated).MappedResponseValidityModel();
            }

            return model.ValidityModel;

        }

        public async Task<PostRegistrationModel> UpdateUserOTP(UserModel userModel, bool isEmailOTPOnly)
        {
            var model = new PostRegistrationModel();
            var entity = Mapper.Map<UserFormModel, UserEntity>(userModel);
            var appSetting = await _appSettingRepository.GetApplicationSetting();

            entity.PhoneNumber = userModel.PhoneNumber;
            entity.OTP = await _iOTPService.GenerateOTP();
            entity.OTPExpirationDate = _iOTPService.OTPExpiration();

            var ret = await _userRepository.UpdateUserOTP(entity);

            if (ret > 0)
            {
                bool isSent = false;
                var refcode = string.Concat(DateTime.Now.Year.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Millisecond.ToString(), entity.PhoneNumber);
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.SMSOTP).MappedResponseValidityModel();
                model.OTP = entity.OTP;
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.Pin, entity.OTP);
                model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.Validity, appSetting.OTPMinutes.ToString());


                if (!isEmailOTPOnly)
                {
                    isSent = await _httpService.SendSMS(entity.PhoneNumber, model.ValidityModel.Message, refcode.ToUpper());

                    if (!isSent)
                    {
                        model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FailedNotification).MappedResponseValidityModel();
                        model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.NotificationType, "SMS");
                    }

                    //else
                    //  model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.NotificationSent).MappedResponseValidityModel();

                }
                //model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.NotificationType, GlobalConstants.MessageReplacement.OTP);

                var SMSTitle = $"{globalConfig.AppName} - NEW OTP Notification";
                var Body = model.ValidityModel.Message;
                //To Send Email Notifications
                bool isSentEmail = await _notificationService.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = SMSTitle,
                    Message = Body,
                    To = userModel.Email,
                    Type = MessageType.HtmlContent
                });


                if (!isSentEmail)
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FailedNotification).MappedResponseValidityModel();
                    model.ValidityModel.Message = model.ValidityModel.Message.Replace(MessageParameters.NotificationType, "Email");
                }

                else
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.NotificationSent).MappedResponseValidityModel();


            }

            return model;
        }


        public async Task<ResponseValidityModel> SetUser(AdminUserFormModel adminUserForm)
        {
            if (adminUserForm.AdminUserId == 0)
            {
                var entity = _userRepository.GetAdminUsers().Result.Where(e => e.Email == adminUserForm.Email || e.Username == adminUserForm.Username).FirstOrDefault();

                if (entity != null)
                {
                    var model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(MessageParameters.Field, "Username/Email");

                    return model;
                }
            }
            else
            {
                var current = _userRepository.GetAdminUsers().Result.Where(e => e.AdminUserId == adminUserForm.AdminUserId).FirstOrDefault();
                if (current.Email != adminUserForm.Email)// || current.Username != adminUserForm.Username)
                {
                    var entity = _userRepository.GetAdminUsers().Result.Where(e => e.Email == adminUserForm.Email).FirstOrDefault();

                    if (entity != null)
                    {
                        var model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                        model.Message = model.Message.Replace(MessageParameters.Field, "Email");

                        return model;
                    }
                }

                if (current.Username != adminUserForm.Username)
                {
                    var entity = _userRepository.GetAdminUsers().Result.Where(e => e.Username == adminUserForm.Username).FirstOrDefault();

                    if (entity != null)
                    {
                        var model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                        model.Message = model.Message.Replace(MessageParameters.Field, "Username");

                        return model;
                    }
                }
            }

            var form = Mapper.Map<AdminUserFormModel, AdminUserEntity>(adminUserForm);
            form.Password = Shared.SharedServices.HashPassword(adminUserForm.Password);

            var ret = await _userRepository.SetAdminUserEntity(form);
            string message = "";
            string title = "";
            int returnNum = 0;
            if (ret > 0)
            {
                if (adminUserForm.AdminUserId > 0)
                {
                    //Updating the Roles
                    if (adminUserForm.RoleId != null && adminUserForm.RoleId.Count > 0)
                    {
                        await userRoleService.Save(new UserRoleModel
                        {
                            UserId = adminUserForm.AdminUserId,
                            RoleId = adminUserForm.RoleId
                        });
                    }
                    returnNum = ApplicationMessageNumber.InformationMessage.RecordUpdated;
                    title = $"{globalConfig.AppName} - Update Account Notification";
                    message = await AppMessageService.GetApplicationMessage(ApplicationMessageNumber.EmailMessage.UpdateAdminUser);
                }
                else
                {
                    //Updating the Roles
                    await userRoleService.Save(new UserRoleModel
                    {
                        UserId = ret,
                        RoleId = adminUserForm.RoleId
                    });

                    returnNum = ApplicationMessageNumber.InformationMessage.RecordAdded;
                    title = $"{globalConfig.AppName} - New Account Registration";
                    message = await AppMessageService.GetApplicationMessage(ApplicationMessageNumber.EmailMessage.NewAdminUser);
                }


                message = message.Replace(MessageParameters.Recipient, adminUserForm.FirstName);
                message = message.Replace(MessageParameters.UserName, adminUserForm.Username);
                message = message.Replace(MessageParameters.Password, adminUserForm.Password);

                //To Send Email Notifications
                bool isSentEmail = await _notificationService.SendSmtpEmailAsync(new EmailModel
                {
                    Subject = title,
                    Message = message,
                    To = adminUserForm.Email,
                    Type = MessageType.HtmlContent
                });
                if (isSentEmail)
                    _logger.LogInformation("Success sending of email for user registration with email " + adminUserForm.Email);
                else
                    _logger.LogError("Failed sending of email for user registration with email " + adminUserForm.Email);


                return AppMessageService.SetMessage(returnNum).MappedResponseValidityModel();
            }
            else
            {
                return AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
            }

        }

        public async Task<AdminUserModel> GetAdminUser(string username, bool includeRoles = true)
        {
            var entity = await _userRepository.GetAdminUser(username, null);
            var model = Mapper.Map<AdminUserModel>(entity);
            if (includeRoles)
            {
                var roleIds = await _userRoleRepository.GetUserRoles(model.AdminUserId);
                if (roleIds.Count > 0)
                    model.RoleId = roleIds.Select(s => s.RoleId).ToList();
            }
            return model;
        }
        public async Task<List<AdminUserModel>> GetAdminUsers(string userName = null, string email = null)
        {
            var entity = await _userRepository.GetAdminUsers(userName, email);
            var model = Mapper.Map<List<AdminUserModel>>(entity);
            foreach (var user in model)
            {
                var roleIds = await _userRoleRepository.GetUserRoles(user.AdminUserId);
                if (roleIds.Count > 0)
                    user.RoleId = roleIds.Select(s => s.RoleId).ToList();
            }
            return model;
        }
        public async Task<ResponseValidityModel> SaveUserSubscription(UserSubscriptionModel param, string userkeyId = null)
        {
            var model = ValidateUserSubscription(param);

            if (model.MessageReturnNumber == 0)
            {
                param.ExpiryDate = param.ExpiryDate;
                param.NextBillingDate = param.NextBillingDate;
                var entity = Mapper.Map<UserSubscriptionModel, UserSubscriptionEntity>(param);
                var response = await userSubscriptionRepository.Save(entity, userkeyId);
                if (response.Item1 > 0)
                {
                    //         JobScheduler.Schedule<IUserService>(
                    //(s) => s.UserSubscriptionNotificationBeforeExpire(response.Item1),
                    //entity.ExpiryDate.AddDays(-1));
                }
                model = AppMessageService.SetMessage(response.Item2).MappedResponseValidityModel();
            }

            model = AppMessageService.SetMessage(model.MessageReturnNumber).MappedResponseValidityModel();

            return model;
        }
        ResponseValidityModel ValidateUserSubscription(UserSubscriptionModel param)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(param.UserKeyId) || param.LockerDetailId < 1 ||
                param.CabinetLocationId < 1 || param.Price < 1 || param.LockerTypeId < 1)
            {
                model.MessageReturnNumber = 1;
                return model;
            }

            var dbModel = lockerRepository.GetLockerBookingByTransactionId(param.LockerTransactionsId).Result;
            if (dbModel == null)
            {
                model.MessageReturnNumber = GlobalConstants.ApplicationMessageNumber.ErrorMessage.LockerTransactionIdForSubscription;
                return model;
            }
            else if (dbModel.UserKeyId != param.UserKeyId)
            {
                model.MessageReturnNumber = GlobalConstants.ApplicationMessageNumber.ErrorMessage.LockerTransactionUnavailableForForSubscription;
                return model;
            }
            else if (dbModel.BookingStatus == BookingTransactionStatus.Confiscated.GetHashCode() ||
                     dbModel.BookingStatus == BookingTransactionStatus.Completed.GetHashCode() ||
                     dbModel.BookingStatus == BookingTransactionStatus.Reassigned.GetHashCode())
            {
                model.MessageReturnNumber = GlobalConstants.ApplicationMessageNumber.ErrorMessage.LockerExpiredForForSubscription;
                return model;
            }

            return model;
        }

        public async Task<ResponseValidityModel> SaveCharges(ChargesModel param)
        {
            var model = ValidateCharges(param);
            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<ChargesModel, ChargesEntity>(param);
                var ret = await chargesRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidateCharges(ChargesModel param)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(param.UserKeyId) || param.LockerDetailId < 1 ||
                param.LockerTypeId < 1)
                model.MessageReturnNumber = 1;
            return model;
        }
        public async Task<List<ChargesViewModel>> GetChargesByUser(string userKeyId, string paymentRefNo = null)
        {
            var dbModel = await chargesRepository.GetChargesByUserKeyId(userKeyId, paymentRefNo);
            var model = new List<ChargesViewModel>();
            if (dbModel != null)
                model = Mapper.Map<List<ChargesViewModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> DeleteCharges(int id)
        {
            var model = new ResponseValidityModel();
            if (id > 0)
            {
                var ret = await chargesRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
        public async Task<List<UserSubscriptionViewModel>> GetSubscriptionsByUser(string userKeyId)
        {
            var dbModel = await userSubscriptionRepository.GetUserSubscription(userKeyId);
            var model = new List<UserSubscriptionViewModel>();

            if (dbModel != null)
                model = Mapper.Map<List<UserSubscriptionViewModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> DeleteUserSubscription(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await userSubscriptionRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
        public async Task ExpiredLockerBookingsNotifications()
        {
            if (hangfireConfig.PushNotificationEnable
                || hangfireConfig.EmailNotificationEnable)
            {
                var expiredSubscriptions = await lockerRepository.GetExpiredLockerBookings();
                if (expiredSubscriptions.Count < 1) return;
                var notifiedBookingTransactions = new List<int>();
                var expiredMessageTitle = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpired).MappedResponseValidityModel()).Message;
                var expiredLongMessageBody = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpiredLongMessage).MappedResponseValidityModel()).Message;
                // To Send Email
                var uniqueTransactions = expiredSubscriptions.GroupBy(s => s.LockerTransactionsId).ToList();
                foreach (var item in uniqueTransactions)
                {
                    var first = item.First();
                    if (first == null) continue;
                    expiredLongMessageBody = expiredLongMessageBody.Replace(MessageParameters.BookingTypeDescription, first.BookingStatus == BookingTransactionStatus.ForDropOff ? $"Drop Off code {first.DropOffCode ?? ""}" : $"Collect code {first.PickUpCode ?? ""}");
                    expiredLongMessageBody = expiredLongMessageBody.Replace(MessageParameters.Refcode, first.PaymentReference.ToUpper());
                    await _notificationService.SendSmtpEmailAsync(new EmailModel
                    {
                        Message = expiredLongMessageBody,
                        Subject = expiredMessageTitle,
                        To = first.Email,
                        Type = MessageType.PlainTextContent
                    });
                    notifiedBookingTransactions.Add(first.LockerTransactionsId);
                }
                var expiredShortMessageBody = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpiredShortMessage).MappedResponseValidityModel()).Message;
                // To Send Push Notification to all devices
                foreach (var transaction in expiredSubscriptions)
                {
                    expiredShortMessageBody = expiredShortMessageBody.Replace(MessageParameters.BookingTypeDescription, transaction.BookingStatus == BookingTransactionStatus.ForDropOff ? $"Drop Off code {transaction.DropOffCode ?? ""}" : $"Collect code {transaction.PickUpCode ?? ""}");
                    expiredShortMessageBody = expiredShortMessageBody.Replace(MessageParameters.Refcode, transaction.PaymentReference.ToUpper());
                    await _notificationService.SendFCMNotificationAsync(new FCMNotificationRequest
                    {
                        deviceType = transaction.DeviceType,
                        token = transaction.Token,
                        clickAction = "OPEN_APP",
                        title = expiredMessageTitle,
                        notificationBody = expiredShortMessageBody,
                        type = "subscription_expire_in_an_hour",
                        json = JsonSerializer.Serialize(new ShortLockerTransactionModel { lockerDetailId = transaction.LockerDetailId, lockerTransactionId = transaction.LockerTransactionsId, startDate = transaction.StoragePeriodStart, endDate = transaction.StoragePeriodEnd, accessPlan = transaction.AccessPlan })
                    });
                }
                if (notifiedBookingTransactions.Count > 0)
                    await lockerRepository.UpdateNotifiedLockerBookings(notifiedBookingTransactions);
            }
        }

        public async Task UserExpiredBookingNotificaiton(int id)
        {
            _logger.LogInformation($"Expired Booking Notification triggered with LockerTransactionsId {id}");
            var lockerTransactionWithUserDetails = await lockerRepository.GetAtiveLockerBookingDetail(id);
            if (lockerTransactionWithUserDetails.Count < 1) return;
            var first = lockerTransactionWithUserDetails.First();
            var lockerDetails = await lockerRepository.GetLockerDetail(first.LockerDetailId);
            if (lockerDetails.Count < 1) return;
            var currentLocker = lockerDetails.First();

            if (currentLocker.BookingStatus == 1  && string.IsNullOrEmpty(first.PickUpCode) == false)
            {
                //booking is completed. dropOff then pickup
                await lockerRepository.UpdateBookingStatus(first.LockerTransactionsId, (int)GlobalEnums.BookingTransactionStatus.Completed);

                return;
            }

            var userDetail = await _userRepository.GetUserFromEmail(first.Email);
            if (userDetail == null) return;
            double minsLeft = 0;

            var lockerBooking = await lockerRepository.GetLockerBooking(id);

            if (!lockerBooking.NewStoragePeriodEndDate.HasValue)
                 minsLeft = first.StoragePeriodEnd.Subtract(DateTime.Now).TotalMinutes;
            else
            {
                minsLeft =  ((DateTime)(lockerBooking.NewStoragePeriodEndDate)).Subtract(DateTime.Now).TotalMinutes;
            }

            if (minsLeft <= 0)
            {
                //validate if collected

                _logger.LogInformation($"Expired Booking Notification passed date with LockerTransactionsId {id} and mins past {minsLeft}");

                var notifiedBookingTransactions = new List<int>();
                    var expiredMessageTitle = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpired).MappedResponseValidityModel()).Message;
                    var expiredLongMessageBody = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpiredLongMessage).MappedResponseValidityModel()).Message;
                    var refCode = first.PaymentReference;
                    // To Send Email
                    expiredLongMessageBody = expiredLongMessageBody.Replace(MessageParameters.BookingTypeDescription, first.BookingStatus == BookingTransactionStatus.ForDropOff ? $"Drop Off code {first.DropOffCode ?? ""}" : $"Collect code {first.PickUpCode ?? ""}");
                    expiredLongMessageBody = expiredLongMessageBody.Replace(MessageParameters.Refcode, refCode.ToUpper());
                    expiredLongMessageBody = expiredLongMessageBody.Replace(MessageParameters.LockerNumber, first.LockerNumber);
                    expiredLongMessageBody = expiredLongMessageBody.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                    expiredLongMessageBody = expiredLongMessageBody.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                                        
                    await _notificationService.SendSmtpEmailAsync(new EmailModel
                    {
                        Message = expiredLongMessageBody,
                        Subject = expiredMessageTitle,
                        To = first.Email,
                        Type = MessageType.PlainTextContent
                    });
                    notifiedBookingTransactions.Add(first.LockerTransactionsId);

                    var expiredShortMessageBody = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpiredShortMessage).MappedResponseValidityModel()).Message;
                    expiredShortMessageBody = expiredShortMessageBody.Replace(MessageParameters.BookingTypeDescription, first.BookingStatus == BookingTransactionStatus.ForDropOff ? $"Drop Off code {first.DropOffCode ?? ""}" : $"Collect code {first.PickUpCode ?? ""}");
                    expiredShortMessageBody = expiredShortMessageBody.Replace(MessageParameters.Refcode, refCode.ToUpper());
                    // To Send Push Notification to all devices

                    await _notificationService.SendFCMNotificationAsync(new FCMNotificationRequest
                    {
                        deviceType = first.DeviceType,
                        token = first.Token,
                        clickAction = "OPEN_APP",
                        title = expiredMessageTitle,
                        notificationBody = expiredShortMessageBody,
                        type = "subscription_expire_in_an_hour",
                        json = JsonSerializer.Serialize(new ShortLockerTransactionModel { lockerDetailId = first.LockerDetailId, lockerTransactionId = first.LockerTransactionsId, startDate = first.StoragePeriodStart, endDate = first.StoragePeriodEnd,
                        accessPlan = first.AccessPlan
                        })
                    });

                    if (notifiedBookingTransactions.Count > 0)
                    {
                        await lockerRepository.UpdateNotifiedLockerBookings(notifiedBookingTransactions);
                    
                    }

                    //Send SMS
                 
                    var isSent = await _httpService.SendSMS(userDetail.PhoneNumber, expiredLongMessageBody, refCode.ToUpper());
                    if (isSent)
                        _logger.LogInformation($"Success sending of SMS for last booking expired having {refCode} to user with Phone Number " + userDetail.PhoneNumber);
                    else
                        _logger.LogError($"Failed sending of SMS for booking expired having {refCode} to user with Phone Number " + userDetail.PhoneNumber);
                     _logger.LogInformation($"Expired Booking Notification triggered with LockerTransactionsId {id}");

                await lockerRepository.UpdateBookingStatus(first.LockerTransactionsId, (int)GlobalEnums.BookingTransactionStatus.Expired);

            }


        }
        public async Task UserLastHourSubscriptionNotification(int id)
        {
            var lockerTransactionWithUserDetails = await lockerRepository.GetAtiveLockerBookingDetail(id);
            
            if (lockerTransactionWithUserDetails.Count < 1) return;


            var first = lockerTransactionWithUserDetails.First();
            var lockerDetails = await lockerRepository.GetLockerDetail(first.LockerDetailId);
            if (lockerDetails.Count < 1) return;
            var currentLocker = lockerDetails.First();
            var userDetail = await _userRepository.GetUserFromEmail(first.Email);
            if (userDetail == null) return;
      

            var hrsLeft = first.StoragePeriodEnd.Subtract(DateTime.Now).TotalHours;
            var refCode = first.PaymentReference;
            if (hrsLeft > 0 && hrsLeft <= 2)
            {
                string timeIntv = String.Empty;
                var hrsLeftInt = (int)hrsLeft;
                if (hrsLeftInt == 0) {
                    var intTerval = ((int)first.StoragePeriodEnd.Subtract(DateTime.Now).TotalMinutes);

                    timeIntv = intTerval > 1 ? intTerval.ToString() + " mins " : intTerval.ToString () + " min";
                 
                }
                else
                    timeIntv = (hrsLeftInt).ToString() + (hrsLeftInt > 1 ? " hrs" : " hr");

                var expireTodayTitle = "Locker subscription reminder!";
                var expireTodayLongMsgBody = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpireTodayLongMsg).MappedResponseValidityModel()).Message;
                expireTodayLongMsgBody = expireTodayLongMsgBody.Replace(MessageParameters.HoursTxt, timeIntv);
                expireTodayLongMsgBody = expireTodayLongMsgBody.Replace(MessageParameters.Refcode, refCode.ToUpper());
                expireTodayLongMsgBody = expireTodayLongMsgBody.Replace(MessageParameters.PlanType, first.AccessPlan ==2 ? "multi access" : "single access"); ;
                expireTodayLongMsgBody = expireTodayLongMsgBody.Replace(MessageParameters.LockerNumber, first.LockerNumber);
                expireTodayLongMsgBody = expireTodayLongMsgBody.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                expireTodayLongMsgBody = expireTodayLongMsgBody.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                expireTodayLongMsgBody = expireTodayLongMsgBody.Replace("{DropOffCode}", first.DropOffCode);
                expireTodayLongMsgBody = expireTodayLongMsgBody.Replace("{PickUpCode}", first.PickUpCode);
                //To Send Email Notifications
                await _notificationService.SendSmtpEmailAsync(new EmailModel
                {
                    Message = expireTodayLongMsgBody.Replace("'", "\""),
                    Subject = expireTodayTitle,
                    To = first.Email,
                    Type = MessageType.HtmlContent
                }); ;

                var expireTodayShortMsgBody = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpireTodayShortMsg).MappedResponseValidityModel()).Message;
                expireTodayShortMsgBody = expireTodayShortMsgBody.Replace(MessageParameters.HoursTxt, timeIntv);
                expireTodayShortMsgBody = expireTodayShortMsgBody.Replace(MessageParameters.Refcode, refCode.ToUpper());
                expireTodayShortMsgBody = expireTodayShortMsgBody.Replace("{DropOffCode}", first.DropOffCode);
                expireTodayShortMsgBody = expireTodayShortMsgBody.Replace("{PickUpCode}", first.PickUpCode);
                // To Send Push Notification to all devices
                foreach (var transaction in lockerTransactionWithUserDetails)
                {
                    await _notificationService.SendFCMNotificationAsync(new FCMNotificationRequest
                    {
                        deviceType = transaction.DeviceType,
                        token = transaction.Token,
                        clickAction = "OPEN_APP",
                        title = expireTodayTitle,
                        notificationBody = expireTodayShortMsgBody,
                        type = "subscription_expire_in_an_hour",
                        json = JsonSerializer.Serialize(new ShortLockerTransactionModel { lockerDetailId = transaction.LockerDetailId,
                            lockerTransactionId = transaction.LockerTransactionsId, startDate = transaction.StoragePeriodStart, 
                            endDate = transaction.StoragePeriodEnd, CabinetLocationDescription = transaction.CabinetLocationDescription, 
                            LockerNumber= transaction.LockerNumber,
                            accessPlan = transaction.AccessPlan})
                    });
                }

                //Send SMS
                var isSent = await _httpService.SendSMS(userDetail.PhoneNumber, expireTodayLongMsgBody, refCode);
                if (isSent)
                    _logger.LogInformation($"Success sending of SMS for last hour warning having {refCode} to user with Phone Number " + userDetail.PhoneNumber);
                else
                    _logger.LogError($"Failed sending of SMS for last hour warning  having {refCode} to user with Phone Number " + userDetail.PhoneNumber);


            }
        }
        public async Task UserLastWeekSubscriptionNotification(int id)
        {
            var getLockerTransactionWithUserDetails = await lockerRepository.GetAtiveLockerBookingDetail(id);
            if (getLockerTransactionWithUserDetails.Count < 1) return;
            var first = getLockerTransactionWithUserDetails.First();

            var userDetail = await _userRepository.GetUserFromEmail(first.Email);
            if (userDetail == null) return;

            var lockerDetails = await lockerRepository.GetLockerDetail(first.LockerDetailId);
            if (lockerDetails.Count < 1) return;
            var currentLocker = lockerDetails.First();
            var daysLeft = first.StoragePeriodEnd.Subtract(DateTime.Now).TotalDays;
            var refCode = first.PaymentReference;
            if (daysLeft > 0 && daysLeft <= 8)
            {
                var expireLastWeekTitle = "Locker subscription reminder!";
                var expireSoonEmailMsg = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpireSoonLongMsg).MappedResponseValidityModel()).Message;
                expireSoonEmailMsg = expireSoonEmailMsg.Replace(MessageParameters.NoOfDays, ((int)daysLeft).ToString());
                expireSoonEmailMsg = expireSoonEmailMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                expireSoonEmailMsg = expireSoonEmailMsg.Replace(MessageParameters.PlanType, first.AccessPlan == 2 ? "multi access" : "single access");
                expireSoonEmailMsg = expireSoonEmailMsg.Replace(MessageParameters.LockerNumber, first.LockerNumber);
                expireSoonEmailMsg = expireSoonEmailMsg.Replace(MessageParameters.LockerName, currentLocker.CabinetLocationDescription);
                expireSoonEmailMsg = expireSoonEmailMsg.Replace(MessageParameters.LockerAddress, currentLocker.CabinetLocationAddress);
                expireSoonEmailMsg = expireSoonEmailMsg.Replace("{DropOffCode}", first.DropOffCode);
                //To Send Email Notifications
                await _notificationService.SendSmtpEmailAsync(new EmailModel
                {
                    Message = expireSoonEmailMsg,
                    Subject = expireLastWeekTitle,
                    To = first.Email,
                    Type = MessageType.PlainTextContent
                });
                var expireSoonShortMsg = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LockerSubscriptionExpireSoonShortMsg).MappedResponseValidityModel()).Message;
                expireSoonShortMsg = expireSoonShortMsg.Replace(MessageParameters.NoOfDays, ((int)daysLeft).ToString());
                expireSoonShortMsg = expireSoonShortMsg.Replace(MessageParameters.Refcode, refCode.ToUpper());
                expireSoonShortMsg = expireSoonShortMsg.Replace("{DropOffCode}", first.DropOffCode);
                // To Send Push Notification to all devices
                foreach (var transaction in getLockerTransactionWithUserDetails)
                {
                    await _notificationService.SendFCMNotificationAsync(new FCMNotificationRequest
                    {
                        deviceType = transaction.DeviceType,
                        token = transaction.Token,
                        clickAction = "OPEN_APP",
                        title = expireLastWeekTitle,
                        notificationBody = expireSoonShortMsg,
                        type = "subscription_expire_in_one_week",
                        json = JsonSerializer.Serialize(new ShortLockerTransactionModel { lockerDetailId = transaction.LockerDetailId,
                            lockerTransactionId = transaction.LockerTransactionsId, startDate = transaction.StoragePeriodStart, 
                            endDate = transaction.StoragePeriodEnd,
                            CabinetLocationDescription = transaction.CabinetLocationDescription,
                            LockerNumber = transaction.LockerNumber,
                            accessPlan= transaction.AccessPlan
                        })
                    });
                }

                //Send SMS
                var isSent = await _httpService.SendSMS(userDetail.PhoneNumber, expireSoonEmailMsg, refCode);
                if (isSent)
                    _logger.LogInformation($"Success sending of SMS for last hour warning having {refCode} to user with Phone Number " + userDetail.PhoneNumber);
                else
                    _logger.LogError($"Failed sending of SMS for last hour warning  having {refCode} to user with Phone Number " + userDetail.PhoneNumber);


            }
        }

        #region User Subscription Billing
        public async Task<ResponseValidityModel> SaveUserSubscriptionBilling(UserSubscriptionBillingModel param)
        {
            var model = ValidateUserSubscriptionBilling(param);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<UserSubscriptionBillingModel, UserSubscriptionBillingEntity>(param);
                var ret = await userSubscriptionBillingRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidateUserSubscriptionBilling(UserSubscriptionBillingModel param)
        {
            var model = new ResponseValidityModel();
            if (param.UserSubscriptionId < 1 || param.PaidAmount < 1)
                model.MessageReturnNumber = 1;

            return model;
        }
        public async Task<List<UserSubscriptionBillingModel>> GetUserSubscriptionBillingBySubscriptionId(int userSubscriptionId)
        {
            var dbModel = await userSubscriptionBillingRepository.GetUserSubscriptionBillingBySubscriptionId(userSubscriptionId);
            var model = new List<UserSubscriptionBillingModel>();

            if (dbModel != null)
                model = Mapper.Map<List<UserSubscriptionBillingModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> DeleteUserSubscriptionBilling(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await userSubscriptionBillingRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<ResponseValidityModel> DeleteAdminUser(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await _userRepository.DeleteAdminUserEntity(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<ResponseValidityModel> DeleteAppUser(string userKeyId)
        {
            var model = new ResponseValidityModel();

            if (userKeyId != "")
            {
                var ret = await _userRepository.DeleteAppUserEntity(userKeyId);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
        #endregion
        #region User Token
        public async Task<ResponseValidityModel> SaveUserToken(UserTokenModel param, string userKeyId, string deviceDescription)
        {
            var model = ValidateUserToken(param);
            if (string.IsNullOrEmpty(userKeyId)) return model;
            var userDetail = await _userRepository.GetUserByUserKeyId(userKeyId);
            if (userDetail == null) return model;
            var existing = await userTokenRepository.GetUserTokenByUserId(userDetail.UserId, deviceDescription, param.DeviceType);
            if (model.MessageReturnNumber == 0)
            {
                if (existing.Count > 0)
                {
                    var oldRecord = existing.First();
                    if (oldRecord.Token != param.Token)
                    {
                        var ret = await userTokenRepository.Save(new UserTokenEntity
                        {
                            Id = oldRecord.Id,
                            Description = oldRecord.Description,
                            DeviceType = oldRecord.DeviceType,
                            Token = param.Token,
                            IsEnable = true,
                            UserId = userDetail.UserId
                        });
                        model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
                    }
                }
                else
                {
                    var ret = await userTokenRepository.Save(new UserTokenEntity
                    {
                        Description = deviceDescription,
                        DeviceType = param.DeviceType,
                        Token = param.Token,
                        IsEnable = true,
                        UserId = userDetail.UserId
                    });
                    model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
                }
            }
            return model;
        }
        ResponseValidityModel ValidateUserToken(UserTokenModel param)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(param.Token) || !Enum.IsDefined(typeof(DeviceType), param.DeviceType))
                model.MessageReturnNumber = 1;

            return model;
        }
        public async Task<List<UserTokenModel>> GetUserTokenByUserId(string userKeyId, string deviceDescription = "", DeviceType? deviceType = null)
        {
            var model = new List<UserTokenModel>();
            if (string.IsNullOrEmpty(userKeyId)) return model;
            var userDetail = await _userRepository.GetUserByUserKeyId(userKeyId);
            if (userDetail == null) return model;
            var dbModel = await userTokenRepository.GetUserTokenByUserId(userDetail.UserId, deviceDescription, deviceType);

            if (dbModel != null)
                model = Mapper.Map<List<UserTokenModel>>(dbModel);
            return model;
        }

        #endregion


        public async Task<ResponseValidityModel> CreateUserFavouriteCabinetLocations(string userKeyId, int cabinetLocartionId)
        {
            var model = new ResponseValidityModel();
            if (!string.IsNullOrEmpty(userKeyId) || cabinetLocartionId == 0)
            {
                var existing = _userRepository.GetUserFavoritesCabinetLocationsList(userKeyId).Result
                     .Where(x => x.CabinetLocationId == cabinetLocartionId && x.UserKeyId == userKeyId).ToList();

                if (existing.Count == 0)
                {

                    var ret = await _userRepository.CreateUserFavouriteCabinetLocations(userKeyId, cabinetLocartionId);
                    model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
                }
                else
                {
                    model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, "CabinetLocationId");
                }

            }

            return model;
          
        }

        public async Task<ResponseValidityModel> DeleteUserFavouriteCabinetLocations(int Id)
        {
            var model = new ResponseValidityModel();
            if (Id > 0)
            {
                var ret = await _userRepository.DeleteUserFavouriteCabinetLocations(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<List<CabinetLocationViewModel>> GetUserFavoritesCabinetLocations(string userKeyId)
        {
            var dbModel = await _userRepository.GetUserFavoritesCabinetLocations(userKeyId);
            var model = new List<CabinetLocationViewModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetLocationViewModel>>(dbModel);
           
            }

            return model;

        }


    }
}
