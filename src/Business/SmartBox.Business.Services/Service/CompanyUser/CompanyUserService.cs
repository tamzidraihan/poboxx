using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.CompanyUser;
using SmartBox.Business.Core.Models.CompanyUser;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.ApplicationSettings;
using SmartBox.Infrastructure.Data.Repository.Company;
using SmartBox.Infrastructure.Data.Repository.CompanyUser;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.CompanyUser
{
    public class CompanyUserService : BaseMessageService<CompanyUserService>, ICompanyUserService
    {
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IAppMessageService _appMessageService;
        private readonly IApplicationSettingRepository _applicationSettingRepository;
        private readonly IConfiguration _configuration;

        public CompanyUserService(IAppMessageService appMessageService, IMapper mapper,
                                  ICompanyUserRepository companyUserRepository,
                                  IApplicationSettingRepository applicationSettingRepository,
                                  ICompanyRepository companyRepository, IConfiguration configuration) : base(appMessageService, mapper)
        {
            _companyUserRepository = companyUserRepository;
            _appMessageService = appMessageService;
            _applicationSettingRepository = applicationSettingRepository;
            _companyRepository = companyRepository;
            _configuration = configuration;
        }

        async Task<ResponseValidityModel> ValidateBaseFields(BaseCompanyUserModel baseCompanyUserModel, bool isInsert)
        {
            var model = new ResponseValidityModel();

            bool isExisting = _companyRepository.CheckCompanyIfExists(companyId: baseCompanyUserModel.CompanyId);

            if (!isExisting)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.CompanyId);
                return model;
            }

            if (!baseCompanyUserModel.FirstName.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.UserFormModel.FirstName);
            }

            if (!baseCompanyUserModel.LastName.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.UserFormModel.LastName);
            }

            if (!baseCompanyUserModel.Email.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.UserFormModel.Email);
            }

            if (!baseCompanyUserModel.Username.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.UserFormModel.Username);
            }

            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                              .MappedResponseValidityModel(model.MessagesList);
                return model;
            }

            var user = await _companyUserRepository.GetCompanyUser(username: baseCompanyUserModel.Username);
            var singleUser = user.FirstOrDefault();

            if (singleUser != null && isInsert)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Username);
                return model;
            }
            else if (singleUser == null && !isInsert)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Username);
                return model;
            }

            if (isInsert)
            {
                user = await _companyUserRepository.GetCompanyUser(email: baseCompanyUserModel.Email);
                if (user.Count > 0)
                {
                    model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Email);
                    return model;
                }
            }
            else
            {
                if (singleUser.Email != baseCompanyUserModel.Email)
                {
                    user = await _companyUserRepository.GetCompanyUser(email: baseCompanyUserModel.Email);
                    if (user.Count > 0)
                    {
                        model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                        model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Email);
                        return model;
                    }
                }
            }

            return model;
        }

        async Task<ResponseValidityModel> ValidateActivationUser(ActivatonCompanyUserModel activatonCompanyUserModel, bool isSystemGenerated)
        {
            var model = new ResponseValidityModel
            {
                MessagesList = new List<string>()
            };

            var entity = new List<CompanyUserEntity>();
            if (isSystemGenerated)
                entity = await _companyUserRepository.GetCompanyUser(userKeyId: activatonCompanyUserModel.UserKeyId);
            else
                entity = await _companyUserRepository.GetCompanyUser(username: activatonCompanyUserModel.Username);

            var entityKey = entity.FirstOrDefault();
            if (entityKey == null)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, string.Concat(GlobalConstants.MessageReplacement.UserKeyId, " or ", GlobalConstants.MessageReplacement.Username));
                return model;
            }
            else if (entityKey.IsActivated)
            {
                if (isSystemGenerated)
                {
                    model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UserAlreadyActivated).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.UserKeyId);
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Id, activatonCompanyUserModel.UserKeyId);
                    return model;
                }
                else
                {
                    model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UserAlreadyActivated).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Username);
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Id, activatonCompanyUserModel.Username);
                    return model;
                }
            }

            if (isSystemGenerated)
            {
                if (!activatonCompanyUserModel.Email.HasText())
                {
                    model.MessagesList.Add(GlobalMessageView.UserFormModel.Email);
                }
                if (!activatonCompanyUserModel.FirstName.HasText())
                {
                    model.MessagesList.Add(GlobalMessageView.UserFormModel.FirstName);
                }
                if (!activatonCompanyUserModel.LastName.HasText())
                {
                    model.MessagesList.Add(GlobalMessageView.UserFormModel.LastName);
                }
                if (!activatonCompanyUserModel.Username.HasText())
                {
                    model.MessagesList.Add(GlobalMessageView.UserFormModel.Username);
                }
                if (!activatonCompanyUserModel.UserKeyId.HasText())
                {
                    model.MessagesList.Add(GlobalMessageView.UserFormModel.UserKeyId);
                }
                if (!activatonCompanyUserModel.Password.HasText())
                {
                    model.MessagesList.Add(GlobalMessageView.UserFormModel.Password);
                }
                if (model.MessagesList.Count > 0)
                {
                    model = this.AppMessageService.SetMessage(-601).MappedResponseValidityModel(model.MessagesList);
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, model.MessagesList.Count > 1 ? Shared.SharedServices.SetVerbMessage(true) : Shared.SharedServices.SetVerbMessage(false));
                    return model;
                }

                var dbmodel = await _companyUserRepository.GetCompanyUser(email: activatonCompanyUserModel.Email);
                var user = dbmodel.FirstOrDefault();
                if (user != null)
                {
                    model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Email);
                    return model;
                }

                dbmodel = await _companyUserRepository.GetCompanyUser(username: activatonCompanyUserModel.Username);
                user = dbmodel.FirstOrDefault();
                if (user != null)
                {
                    model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Username);
                    return model;
                }

            }

            if (!isSystemGenerated)
            {
                if (SharedServices.VerifyPassword(activatonCompanyUserModel.Password, entityKey.Password))
                {
                    model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnchangeDefaultValue).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Password);
                    return model;
                }
            }
            var setting = await _applicationSettingRepository.GetApplicationSetting();
            if (SharedServices.VerifyPassword(activatonCompanyUserModel.Password, setting.DefaultCompanyPassword))
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnchangeDefaultValue).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Password);
                return model;
            }

            //string.Equals(activatonCompanyUserModel.FirstName, "Default", StringComparison.OrdinalIgnoreCase)
            if (string.Equals(activatonCompanyUserModel.FirstName, "Default", StringComparison.OrdinalIgnoreCase) || string.Equals(activatonCompanyUserModel.LastName, "Default", StringComparison.OrdinalIgnoreCase))
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnchangeDefaultValue).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.FirstnameOrLastname);
                return model;
            }

            return model;
        }
        async Task<ResponseValidityModel> ValidateCompanyUser(PostCompanyUserModel postCompanyUserModel)
        {
            var model = await ValidateBaseFields(postCompanyUserModel, true);

            if (model.MessageReturnNumber < 0)
            {
                if (!postCompanyUserModel.Password.HasText() && model.MessageReturnNumber == ApplicationMessageNumber.ErrorMessage.FieldExisting)
                {
                    model.MessagesList.Add(GlobalMessageView.UserFormModel.Password);
                }
                return model;
            }
            return model;
        }
        async Task<ResponseValidityModel> ValidateUpdateCompanyUser(PostUpdateCompanyUserModel postUpdateCompanyUserModel)
        {
            var model = await ValidateBaseFields(postUpdateCompanyUserModel, false);

            return model;
        }

        ResponseValidityModel ValidateCompanyUserLogIn(string username, string password, CompanyUserEntity entity)
        {
            var model = new ResponseValidityModel();

            if (entity == null)
            {
                model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.Username);
                return model;
            }
            if (!entity.IsActivated)
            {
                model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnActivatedBackendUser).MappedResponseValidityModel();
                return model;
            }

            if (!entity.IsDeleted)
            {
                var istrue = Shared.SharedServices.VerifyPassword(password, entity.Password);
                if (!istrue)
                {
                    model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.WrongPassword).MappedResponseValidityModel();
                    return model;
                }
            }
            else
            {
                model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingUser).MappedResponseValidityModel();
            }
            return model;
        }
        private List<Claim> GetClaims(CompanyUserEntity user, bool isBackendAdmin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Name, user.LastName),
            };

            if (user.Username.HasText())
                claims.Add(new Claim(ClaimTypes.Name, user.Username));

            claims.Add(isBackendAdmin
                ? new Claim(ClaimTypes.Role, Roles.BackendUser)
                : new Claim(ClaimTypes.Role, GlobalConstants.Roles.BackendUserAdmin));

            return claims;
        }
        public string CreateToken(CompanyUserEntity user, bool isAdmin)
        {

            var jwtsettings = _configuration.GetSection("JwtSettings");
            var claims = GetClaims(user, isAdmin);
            var jwtSecret = System.Environment.GetEnvironmentVariable("JWT_SECRET");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtsettings.GetSection("validIssuer").Value,
                audience: jwtsettings.GetSection("validAudience").Value,
                claims: claims,
                //expires: DateTime.Now.AddMinutes(1440), //change to 24 hrs validity for now
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResponseValidityModel> ActivateCompanyUser(ActivatonCompanyUserModel activatonCompanyUserModel, bool isSystemGenerated)
        {
            var responseValidityModel = await ValidateActivationUser(activatonCompanyUserModel, isSystemGenerated);
            var entity = Mapper.Map<CompanyUserEntity>(activatonCompanyUserModel);

            if (responseValidityModel.MessageReturnNumber == 0)
            {
                entity.IsActivated = true;
                entity.Password = SharedServices.HashPassword(entity.Password);
                var ret = await _companyUserRepository.ActivateCompanyUser(entity, isSystemGenerated);
                responseValidityModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }

            return responseValidityModel;
        }
        public async Task<ResponseValidityModel> SaveCompanyUser(PostCompanyUserModel postCompanyUserModel)
        {
            var entity = Mapper.Map<CompanyUserEntity>(postCompanyUserModel);
            var responseValidityModel = await ValidateCompanyUser(postCompanyUserModel);

            if (!postCompanyUserModel.Password.HasText())
            {
                responseValidityModel.MessagesList.Add(GlobalMessageView.UserFormModel.Password);
                if (responseValidityModel.MessageReturnNumber != ApplicationMessageNumber.ErrorMessage.FieldRequired)
                {
                    responseValidityModel = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                            .MappedResponseValidityModel(responseValidityModel.MessagesList);
                }
            }

            if (responseValidityModel.MessageReturnNumber == 0)
            {
                int ret;
                var id = _companyUserRepository.GetLastIdentity();

                entity.UserKeyId = Shared.SharedServices.GenerateCompanyUserKeyId(id + 1, postCompanyUserModel.FirstName);
                entity.Password = SharedServices.HashPassword(postCompanyUserModel.Password);
                ret = await _companyUserRepository.SaveCompanyUser(entity, true);
                responseValidityModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }

            return responseValidityModel;
        }

        public async Task<ResponseValidityModel> UpdateCompanyUser(PostUpdateCompanyUserModel postUpdateCompanyUserModel)
        {
            var entity = Mapper.Map<CompanyUserEntity>(postUpdateCompanyUserModel);
            var responseValidityModel = await ValidateUpdateCompanyUser(postUpdateCompanyUserModel);

            if (responseValidityModel.MessageReturnNumber == 0)
            {
                int ret;
                ret = await _companyUserRepository.SaveCompanyUser(entity, false);

                responseValidityModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }

            return responseValidityModel;
        }

        public async Task<List<CompanyUserViewModel>> GetCompanyUser(string userKeyId = null, string username = null, string email = null, bool? isDeleted = null, bool? isActivated = null)
        {
            var dbModel = await _companyUserRepository.GetCompanyUser(username: username);
            if (dbModel != null)
            {
                var model = Mapper.Map<List<CompanyUserViewModel>>(dbModel);
                return model;
            }
            else
            {
                //not existing
                return null;
            }
        }

        public async Task<AuthenticationModel> GetCompanyLogInUser(CompanyUserLogInModel logInModel)
        {
            var model = new AuthenticationModel();
            var dbModel = await _companyUserRepository.GetCompanyUser(username: logInModel.Userame);
            var entity = dbModel.FirstOrDefault();
            var responseValidityModel = ValidateCompanyUserLogIn(logInModel.Userame, logInModel.Password, entity);

            model.ValidityModel = responseValidityModel;

            if (responseValidityModel.MessageReturnNumber >= 0)
            {
                model.Token = CreateToken(entity, entity.IsAdmin);
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LogInSuccess).MappedResponseValidityModel();
            }

            return model;
        }
        public async Task<ResponseValidityModel> DeleteCompanyUser(int id)
        {
            var ret = await _companyUserRepository.Delete(id);
            return AppMessageService.SetMessage(ret).MappedResponseValidityModel();
        }

        public async Task<ResponseValidityModel> ActivateDeactivate(int id, int? isActive)
        {
            var ret = await _companyUserRepository.ActivateDeactivate(id, isActive);
            return AppMessageService.SetMessage(ret).MappedResponseValidityModel();
        }
    }
}
