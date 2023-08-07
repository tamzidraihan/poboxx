using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Services.Service.ApplicationSetting;
using SmartBox.Business.Services.Service.Cabinet;
using SmartBox.Business.Services.Service.Locker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SmartBox.Business.Core.Models.Company;
using SmartBox.Business.Services.Service.Company;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CabinetController : ControllerBase
    {
        private readonly ICabinetService _cabinetService;
        private readonly ILockerService _lockerService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly ICompanyService companyService;
        public CabinetController(ICabinetService cabinetService, ICompanyService companyService, ILockerService lockerService, IApplicationSettingService applicationSettingService)
        {
            _cabinetService = cabinetService;
            _lockerService = lockerService;
            _applicationSettingService = applicationSettingService;
            this.companyService = companyService;
        }

        /// <summary>
        /// Get a list of active Cabinet Location
        /// </summary>
        /// <returns> returns list available Cabinet Location</returns>
        /// <response code="200">CabinetLocationViewModel list</response>
        [HttpGet("GetCabinetLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]

        public async Task<ActionResult<List<CabinetLocationViewModel>>> GetCabinetLocation(bool IsMapRegion)
        {
            var model = await _cabinetService.GetActiveCabinetWithLocation(IsMapRegion);
            return Ok(model);
        }

        /// <summary>
        /// Get a list of active Locker
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns> returns list available Cabinet Location</returns>
        /// <response code="200">LockerDetailModel list</response>
        [HttpGet("GetLockerDetail")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]

        public async Task<ActionResult<List<LockerDetailModel>>> GetLockerDetail(int? companyId = null)
        {
            var model = await _lockerService.GetActiveLocker(null, companyId);
            return Ok(model);
        }

        /// <summary>
        /// Get a list of available Locker for booking
        /// </summary>
        /// <param name="cabinetLocationId">selected cabinetlocationid</param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="lockerTypeId"></param>
        /// <param name="positionId"></param>
        /// <returns> returns list available locker based on the parameter supplied</returns>
        /// <response code="200">AvialableLockerGroupingModel list</response>
        [HttpGet("GetAvailableLocker")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]

        public async Task<ActionResult<List<AvialableLockerGroupingModel>>> GetAvailableLocker(int cabinetLocationId, DateTime dateStart, DateTime dateEnd, int? lockerTypeId = null, int? positionId = null)
        {
            var model = await _lockerService.GetUpdatedAvailableLocker(cabinetLocationId, lockerTypeId, dateStart, dateEnd, false, null, null, null, positionId);
            var groupedData = model.GroupBy(s => s.PositionId).Select(s => new AvialableLockerGroupingModel { PositionId = s.Key, Details = s.ToList() }).ToList();
            return Ok(groupedData);
        }

 

        [HttpGet("GetBookingUpdatedPrice")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]

        public async Task<ActionResult<UpdatedAvailableLockerModel>> GetBookingUpdatedPrice(int lockerTransactionId, int lockerDetailId,
                                                                        DateTime startDate, DateTime endDate)
        {
            var model = await _lockerService.GetBookingUpdatedPrice(lockerTransactionId, lockerDetailId, startDate, endDate);
            return Ok(model);
        }
        /// <summary>
        /// Get a list of distinct available Locker
        /// </summary>
        /// <param name="cabinetLocationId">selected cabinetlocationid</param>
        /// <returns> returns list available locker based on the parameter supplied, group by lockerdetail id</returns>
        /// <response code="200">AvailableLockerModel list</response>
        [HttpGet("GetAvailableLockerDetail")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<AvailableLockerModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<AvailableLockerModel>>> GetAvailableLockerDetail(int cabinetLocationId)
        {
            var model = await _lockerService.GetAvailableLocker(cabinetLocationId, null, null, null, DateTime.Today, DateTime.Today, false, null, null, null);
            var list = model.GroupBy(x => x.LockerDetailId)
                                  .Select(g => g.First())
                                  .ToList();

            return Ok(list);
        }

        /// <summary>
        /// Get a list of distinct available Locker Type by Cabinet Id
        /// </summary>
        /// <param name="cabinetLocationId">cabinetLocation  Id</param>
        /// <param name="dateStart">dateStart</param>
        /// <param name="dateEnd">dateEnd</param>
        /// <returns> returns list available locker type based on the parameter supplied, group by locker type id</returns>
        /// <response code="200">AvailableLockerModel list</response>
        [HttpGet("GetAvailableLockerType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(AvailableLockerTypeListModel), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<AvailableLockerTypeListModel>>> GetAvailableLockerType(int cabinetLocationId, DateTime dateStart, DateTime dateEnd)
        {
            //var appSettings = await _applicationSettingService.GetApplicationSetting();
            // var model = new List<AvailableLockerTypeListModel>();
            //var dateStart = DateTime.Today;
            //var dateEnd = DateTime.Today;
            var model = new AvailableLockerTypeListModel();

            //if (id > 0)
            //{
            //    dateStart = dateStart.AddDays(1);
            //    dateEnd = dateEnd.AddDays(1);
            //}

            var dbModel = await _lockerService.GetAvailableDistinctLocker(cabinetLocationId, null, null, null, dateStart, dateEnd, false, null, null);
            //var ret = await _lockerService.GetAvailableDistinctLocker(cabinetLocationId, null, null, null, dateStart, dateEnd, false, null, null); 

            model.Date = dateStart;
            //model.Id = id;

            var q = dbModel.GroupBy(x => new { x.LockerTypeId, x.LockerTypeDescription, x.Size, x.Price, x.PositionId })
                    .Select(g => new AvailableLockerTypeModel
                    {
                        LockerTypeId = g.Key.LockerTypeId,
                        LockerTypeDescription = g.Key.LockerTypeDescription,
                        Size = g.Key.Size,
                        Price = g.Key.Price,
                        PositionId = g.Key.PositionId,
                        NumberOfAvailable = g.Count()
                    }).ToList();

            model.Collection = q;

            return Ok(model);
        }

        /// <summary>
        /// Get a list of distinct available lower Locker Type by Cabinet Id
        /// </summary>
        /// <param name="cabinetLocationId">cabinetLocation  Id</param>
        /// <param name="dateStart">dateStart</param>
        /// <param name="dateEnd">dateEnd</param>
        /// <returns> returns list available locker type based on the parameter supplied, group by locker type id</returns>
        /// <response code="200">AvailableLockerModel list</response>
        [HttpGet("GetAvailableLowerLockers")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(AvailableLockerTypeListModel), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<AvailableLockerTypeListModel>>> GetAvailableLowerLockers(int cabinetLocationId, DateTime dateStart, DateTime dateEnd)
        {
            //var appSettings = await _applicationSettingService.GetApplicationSetting();
            // var model = new List<AvailableLockerTypeListModel>();
            //var dateStart = DateTime.Today;
            //var dateEnd = DateTime.Today;
            var model = new AvailableLockerTypeListModel();

            //if (id > 0)
            //{
            //    dateStart = dateStart.AddDays(1);
            //    dateEnd = dateEnd.AddDays(1);
            //}

            var dbModel = await _lockerService.GetAvailableDistinctLocker(cabinetLocationId, null, null, null, dateStart, dateEnd, false, null, null);
            //var ret = await _lockerService.GetAvailableDistinctLocker(cabinetLocationId, null, null, null, dateStart, dateEnd, false, null, null); 

            model.Date = dateStart;
            //model.Id = id;

            var q = dbModel.GroupBy(x => new { x.LockerTypeId, x.LockerTypeDescription, x.Size, x.Price })
                      .Select(g => new AvailableLockerTypeModel
                      {
                          LockerTypeId = g.Key.LockerTypeId,
                          LockerTypeDescription = g.Key.LockerTypeDescription,
                          Size = g.Key.Size,
                          Price = g.Key.Price,
                          NumberOfAvailable = g.Count()
                      }).ToList();

            model.Collection = q;

            return Ok(model);
        }


        /// <summary>
        /// Get a list of active locker type (sizes of the locker)
        /// </summary>
        /// <returns> returns list available locker type </returns>
        /// <response code="200">LockerTypeViewModel list</response>
        [HttpGet("GetLockerType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<LockerTypeViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<LockerTypeViewModel>>> GetLockerType()
        {
            var model = await _cabinetService.GetActiveLockerType();
            return Ok(model);
        }

        /// <summary>
        /// Get a list of active cabinet
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns> returns list available cabinet </returns>
        /// <response code="200">CabinetModel list</response>
        [HttpGet("GetCabinet")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<CabinetModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<CabinetModel>>> GetCabinet(int? companyId = null)
        {
            var model = await _cabinetService.GetActiveCabinet(companyId);
            return Ok(model);
        }
        /// <summary>
        /// Get a list of Company cabinet 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="cabinetId"></param>
        /// <param name="unAssignedOnly"></param>
        /// <returns> returns list of company cabinet </returns>
        /// <response code="200">CompanyCabinetViewModel list</response>
        [HttpGet("GetCompanyCabinets")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<CompanyCabinetViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<Task<List<CompanyCabinetViewModel>>>> GetCompanyCabinet(int? companyId = null, int? cabinetId = null, bool? unAssignedOnly = null)
        {
            var model = await companyService.GetCompanyCabinets(companyId, cabinetId, unAssignedOnly);
            return Ok(model);
        }

        /// <summary>
        /// Get a list of active cabinet and it's location
        /// </summary>
        /// <returns> returns list available cabinet </returns>
        /// <response code="200">CabinetViewModel list</response>
        [HttpGet("GetCabinetWithLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<CabinetViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<CabinetViewModel>>> GetCabinetWithLocation(bool IsMapRegion)
        {
            var model = await _cabinetService.GetCabinetWithLocation(IsMapRegion);
            return Ok(model);
        }
        [HttpGet("GetUnavailableLockers")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]

        public async Task<ActionResult<List<LockerDetailModel>>> GetUnavailableLockers(DateTime? startDate = null, DateTime? endDate = null)
        {
            var model = await _lockerService.GetUnavailableLockers(startDate, endDate);
            return Ok(model);
        }
        [HttpPut("ActivateDeactivateCabinetType")]
       // [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> ActivateDeactivateCabinetType([FromQuery] int id, bool isActivated)
        {
            var model = await _cabinetService.ActivateDeactivateCabinetType(id, isActivated);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
    }
}
