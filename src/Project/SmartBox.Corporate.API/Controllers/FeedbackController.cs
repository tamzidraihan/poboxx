using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.ApplicationSetting;
using SmartBox.Business.Services.Service.Feedback;
using SmartBox.Business.Services.Service.Locker;
using System.Net;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILockerService _lockerService;
        private readonly IApplicationSettingService _applicationSettingService;

        public FeedbackController(IFeedbackService feedbackService, ILockerService lockerService, IApplicationSettingService applicationSettingService)
        {
            _feedbackService = feedbackService;
            _lockerService = lockerService;
            _applicationSettingService = applicationSettingService;
        }


        /// <summary>
        /// Add or update a roles type. If the roleId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="feedbackModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreateFeedback")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateFeedback([FromBody] FeedbackModel feedbackModel)
        {
            var model = await _feedbackService.Save(feedbackModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Get a list of roles
        /// </summary>
        /// <returns> returns list roles </returns>
        /// <response code="200">RoleViewModel list</response>  
        [HttpGet("GetFeedback")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public ActionResult<List<FeedbackViewModel>> GetFeedback()
        {
            var model =  _feedbackService.GetAll();
            return Ok(model);
        }

        /// <summary>
        /// Get role by id
        /// </summary>
        /// <returns> role by id </returns>
        /// <response code="200">RoleViewModel list</response>
        [HttpGet("GetFeedbackById")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<FeedbackViewModel>>> GetFeedbackById(int Id)
        {
            var model = await _feedbackService.GetById(Id);
            return Ok(model);
        }

        /// <summary>
        /// Delete role. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteFeedback")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteFeedback([FromQuery] int id)
        {
            var model = await _feedbackService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
    }
}
