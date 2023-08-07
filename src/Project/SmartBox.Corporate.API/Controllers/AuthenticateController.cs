using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Services.Service.LogIn;
using SmartBox.Business.Services.Service.User;
using System.Net;
using System.Threading.Tasks;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ILogInService _logInService;
        private readonly IUserService _userService;
        public AuthenticateController(ILogInService logInService, IUserService userService)
        {
            _logInService = logInService;
            _userService = userService;
        }

        /// <summary>
        /// Log in the user
        /// </summary>
        /// <param name="user">Request's payload</param>
        /// <returns> returns model  with token when success</returns>
        /// <remarks> username and password cannot be empty </remarks>
        /// <response code="200">User successfully log-in</response>
        [HttpPost("LogIn")]
        [ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(AuthenticationModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<AuthenticationModel>> LogIn([BindRequired, FromBody] LogInModel user)
        {
            var model = await _logInService.GetUser(user);
            if (model.ValidityModel.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        /// <summary>
        /// Log in the admin user
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns> returns model  with token when success</returns>
        /// <remarks> username and password cannot be empty </remarks>
        /// <response code="200">User successfully log-in</response>
        [HttpPost("LogInAdminUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(AuthenticationModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<AuthenticationModel>> LogInAdminUser([BindRequired] string username, [BindRequired] string password)
        {
            var model = await _logInService.GetAdminUser(username,password);
            if (model.ValidityModel.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        /// <summary>
        /// Activate the user (not admin user)
        /// </summary>
        /// <param name="phoneNumber">user's phonenumber</param>
        /// <param name="OTP">assigned OTP</param>
        /// <response code="200">User was successfully activated</response>
        [HttpPut("ActivateNewUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(AuthenticationModel), (int)HttpStatusCode.Conflict)]

        public async Task<ActionResult<AuthenticationModel>> ActivateNewUser([BindRequired] string phoneNumber, [BindRequired]  string OTP)
        {
            var model = await _userService.ActivateOTP(phoneNumber, OTP);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

    }    
}
