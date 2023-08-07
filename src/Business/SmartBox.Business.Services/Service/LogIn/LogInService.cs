
using SmartBox.Business.Core.Models.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.AppMessage;
using AutoMapper;
using SmartBox.Infrastructure.Data.Repository.User;
using static SmartBox.Business.Shared.GlobalConstants;
using SmartBox.Business.Core;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Services.Service.User;
using SmartBox.Infrastructure.Data.Repository.AppMessage;
using SmartBox.Business.Shared;

namespace SmartBox.Business.Services.Service.LogIn
{
    public class LogInService : BaseMessageService<LogInService>, ILogInService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;


        public LogInService(IAppMessageService appMessageService, IMapper mapper, IConfiguration configuration,
                            IUserRepository userRepository, IUserService userService) : base(appMessageService, mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userService = userService;

        }

        private List<Claim> GetClaims(UserModel user, bool isAdmin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserKeyId),
                new Claim(ClaimTypes.Name, user.FirstName)
            };

            if (user.PhoneNumber != null)
                claims.Add(new Claim(ClaimTypes.Name, user.PhoneNumber));

            claims.Add(isAdmin
                ? new Claim(ClaimTypes.Role, Roles.Admin)
                : new Claim(ClaimTypes.Role, GlobalConstants.Roles.NonAdministrator));


            return claims;
        }

        public string CreateToken(UserModel user, bool isAdmin)
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


        public async Task<AuthenticationModel> GetUser(LogInModel logInModel)
        {
            var model = new AuthenticationModel();
            var entity = await _userRepository.GetUserFromPhoneNo(logInModel.PhoneNumber);
            var userModel = await _userService.GetUser(logInModel.PhoneNumber);

            if (userModel.ValidityModel.MessageReturnNumber > 0)
            {
                var istrue = Shared.SharedServices.VerifyPassword(logInModel.Password, entity.Password);
                if (!istrue)
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.WrongPassword).MappedResponseValidityModel();

                    return model;
                }
                else
                {
                    var user = this.Mapper.Map<UserEntity, UserModel>(entity);
                    model.Token = CreateToken(user, false);
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LogInSuccess).MappedResponseValidityModel();
                }
            }
            else
            {
                model.ValidityModel = userModel.ValidityModel;
            }

            return model;
        }

        public async Task<AuthenticationModel> GetAdminUser(string username, string password)
        {
            var model = new AuthenticationModel();
            var entity = await _userRepository.GetAdminUser(username, string.Empty);

            if (entity == null)
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.IncorrectUsername).MappedResponseValidityModel();
                return model;
            }

            if (!entity.IsDeleted)
            {
                var istrue = Shared.SharedServices.VerifyPassword(password, entity.Password);
                if (!istrue)
                {
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.WrongPassword).MappedResponseValidityModel();

                    return model;
                }
                else
                {
                    var user = new UserModel();
                    user.UserKeyId = entity.Username;
                    user.FirstName = entity.FirstName;
                    user.Email = entity.Email;

                    model.Token = CreateToken(user, true);
                    model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.LogInSuccess).MappedResponseValidityModel();
                }
            }
            else
            {
                model.ValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InActiveuser).MappedResponseValidityModel();
            }

            return model;
        }
    }
}
