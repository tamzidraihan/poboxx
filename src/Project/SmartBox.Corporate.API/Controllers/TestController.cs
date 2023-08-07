using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SmartBox.Business.Core;
using SmartBox.Business.Services.Service.AppMessage;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Newtonsoft.Json;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Services.Service.Location;
using SmartBox.Business.Core.Models.TextValue;
using static SmartBox.Business.Shared.GlobalConstants;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SmartBox.Business.Services.Service.Cabinet;
using System.Net.Http;
using System.Net.Http.Headers;
using SmartBox.Business.Services.Service.HTTPService;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IAppMessageService _appMessageService;
        private readonly ILocationService _locationService;
        private readonly SMSOptionModel _smsOptionModel;
        private readonly ICabinetService _cabinetService;
        private readonly HttpClient _httpClient;
        private readonly IHttpService _httpService;

        public TestController(IAppMessageService appMessageService, ILocationService locationService,
                             IOptions<SMSOptionModel> smsOptionModel, ICabinetService cabinetService,
                             HttpClient httpClient, IHttpService httpService
                             )
        {
            _appMessageService = appMessageService;
            _locationService = locationService;
            _smsOptionModel = smsOptionModel.Value;
            _cabinetService = cabinetService;
            _httpClient = httpClient;
            _httpService = httpService;
        }

        [HttpGet("TestLocation")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(TextValueHeaderModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<TextValueHeaderModel>> TestLocation(string code)
        {
            var model = await _locationService.GetProvinces(code);
            return Ok(model);
        }

    

      


        [HttpPut("TestAuthorize"), Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        public ActionResult<ResponseValidityModel> TestAuthorize()
        {
            var model = new ResponseValidityModel();
            model.MessagesList.Add("test");
            model.MessagesList.Add("test2");

            model = _appMessageService.SetFailUpdateMessage().MappedResponseValidityModel();
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        [HttpPut("TestCabinet")]
          [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> TestCabinet(int? id)
        {
            var model = new ResponseValidityModel();
            var dbmodel = await _cabinetService.GetCabinetTest(id);

            model = _appMessageService.SetFailUpdateMessage().MappedResponseValidityModel();
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
    }
}
