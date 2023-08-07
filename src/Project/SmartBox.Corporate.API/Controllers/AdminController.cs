using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Models.Announcement;
using SmartBox.Business.Core.Models.ApplicationSetting;
using SmartBox.Business.Core.Models.AppMessage;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.Company;
using SmartBox.Business.Core.Models.Dashboard;
using SmartBox.Business.Core.Models.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Models.FranchiseFeedbackQuestion;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.Logger;
using SmartBox.Business.Core.Models.Logs;
using SmartBox.Business.Core.Models.Maintenance;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.Notification.PushNotification;
using SmartBox.Business.Core.Models.ParentCompany;
using SmartBox.Business.Core.Models.Permission;
using SmartBox.Business.Core.Models.Pricing;
using SmartBox.Business.Core.Models.Report;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Core.Models.RolePermission;
using SmartBox.Business.Core.Models.Roles;
using SmartBox.Business.Core.Models.UserRole;
using SmartBox.Business.Services.Service.Announcement;
using SmartBox.Business.Services.Service.ApplicationSetting;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Cabinet;
using SmartBox.Business.Services.Service.Company;
using SmartBox.Business.Services.Service.Dashboard;
using SmartBox.Business.Services.Service.EmailMessage;
using SmartBox.Business.Services.Service.FranchiseFeedbackQuestion;
using SmartBox.Business.Services.Service.Locker;
using SmartBox.Business.Services.Service.Logs;
using SmartBox.Business.Services.Service.Maintenance;
using SmartBox.Business.Services.Service.Notification;
using SmartBox.Business.Services.Service.ParentCompany;
using SmartBox.Business.Services.Service.Permission;
using SmartBox.Business.Services.Service.Pricing;
using SmartBox.Business.Services.Service.Report;
using SmartBox.Business.Services.Service.Role;
using SmartBox.Business.Services.Service.RolePermission;
using SmartBox.Business.Services.Service.UserRole;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;
using SmartBox.Business.Shared;
using SmartBox.Business.Services.Service.PromoAndDiscounts;
using SmartBox.Business.Core.Models.PromoAndDiscounts;
using SmartBox.Business.Services.Service.Ads;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Ads;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ICabinetService _cabinetService;
        private readonly IPromoAndDiscountsService _promoAndDiscountsService;
        private readonly IAdsService _adsService;
        private readonly ILockerService _lockerService;
        private readonly ICompanyService _companyService;
        private readonly IRoleService _roleService;
        private readonly IUserRoleService _userRoleService;
        private readonly IPermissionService _permissionService;
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IFranchiseFeedbackService _franchiseFeedbackService;
        private readonly IParentCompanyService _parentCompanyService;
        private readonly IPricingService _pricingService;
        private readonly IDashboardService _dashboardService;
        private readonly IAnnouncementService announcementService;
        private readonly IMaintenanceService maintenanceService;
        private readonly INotificationService notificationService;
        private readonly IReportService reportService;
        private readonly IEmailMessageService _emailMessageService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IApplicationMessageService _appMessageService;
        private readonly ILogService _logService;
        public AdminController(ICabinetService cabinetService, ILockerService lockerService, ICompanyService companyService,
                               IRoleService roleService,
                               IUserRoleService userRoleService,
                               IPermissionService permissionService,
                               IRolePermissionService rolePermissionService,
                               IFranchiseFeedbackService franchiseFeedbackService,
                               IParentCompanyService parentCompanyService,
                               IAnnouncementService announcementService,
                               INotificationService notificationService,
                               IPricingService pricingService,
                               IDashboardService dashboardService,
                               IMaintenanceService maintenanceService,
                               IReportService reportService,
                               IEmailMessageService emailMessageService,
                               IApplicationSettingService applicationSettingService,
                               IApplicationMessageService appMessageService,
                               ILogService logService,
                               IPromoAndDiscountsService promoAndDiscountsService,
                               IAdsService adsService
                               )
        {
            _dashboardService = dashboardService;
            _cabinetService = cabinetService;
            _lockerService = lockerService;
            _companyService = companyService;
            _roleService = roleService;
            _permissionService = permissionService;
            _userRoleService = userRoleService;
            _rolePermissionService = rolePermissionService;
            _franchiseFeedbackService = franchiseFeedbackService;
            _parentCompanyService = parentCompanyService;
            _pricingService = pricingService;
            this.maintenanceService = maintenanceService;
            this.announcementService = announcementService;
            this.notificationService = notificationService;
            this.reportService = reportService;
            _emailMessageService = emailMessageService;
            _applicationSettingService = applicationSettingService;
            _appMessageService = appMessageService;
            _logService = logService;
            _promoAndDiscountsService = promoAndDiscountsService;
            _adsService = adsService;
        }

        #region Dashboard
        [HttpGet("GetDashboardData")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public ActionResult<DashboardViewModel> GetDashboardData(int? companyId = null)
        {
            var model = _dashboardService.GetDashboardData(companyId);
            return Ok(model);
        }
        #endregion

        #region Locker Type

        /// <summary>
        /// Get a list of locker type or get by id
        /// </summary>
        /// <returns> returns list locker type </returns>
        /// <response code="200">LockerTypeViewModel list</response>
        [HttpGet("GetLockerType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<LockerTypeViewModel>>> GetLockerType([FromQuery] int?isDeleted=null )
        {
            var model = await _cabinetService.GetLockerType(null,isDeleted);
            return Ok(model);
        }

        /// <summary>
        /// Get a list of locker
        /// </summary>
        /// <returns> returns list locker </returns>
        [HttpGet("GetLockerDetail")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<LockerViewModel>>> GetLockerDetail(int? cabinetId = null, int? companyId = null)
        {
            var model = await _lockerService.GetLocker(null, cabinetId: cabinetId, companyId: companyId);
            return Ok(model);
        }

        /// <summary>
        /// Add or update a locker type. If the lockerTypeId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="lockerType">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveLockerType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveLockerType([FromBody] LockerTypeModel lockerType)
        {
            var model = await _cabinetService.SaveLockerType(lockerType);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }


        /// <summary>
        /// Activate or deactivate the record in database table. Does not delete the record
        /// <br/>
        /// </summary>
        /// <remarks> setting IsDelete = true will deactivate the record </remarks>
        /// <param name="lockerTypeId">CabinetId</param>
        /// <param name="isDeleted">IsDeleted</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPut("LockerTypeActivation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> LockerTypeActivation([BindRequired] int lockerTypeId, [BindRequired] bool isDeleted)
        {
            var model = await _cabinetService.SetLockerTypeActivation(lockerTypeId, isDeleted);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Get a list of distinct available Locker Type by Cabinet Id
        /// </summary>
        /// <param name="dateStart">start date of search base on booking</param>
        /// <param name="dateEnd">end date of search base on booking</param>
        /// <param name="cabinetLocationId">cabinetLocation Id</param>
        /// <param name="cabinetId">cabinet Id</param>
        /// <param name="lockerTypeId">locker type id - to determined the size</param>
        /// <param name="currentPage">the page number</param>
        /// <param name="pageSize">number of records to return</param>
        /// <param name="positionId"></param>
        /// <returns> returns list available locker type based on the parameter supplied, group by locker type id</returns>
        /// <response code="200">AvailableLockerModel list</response>
        [HttpGet("GetAvailableLockerType")]
    //    [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<AvailableLockerTypeListModel>>> GetAvailableLockerType([BindRequired] DateTime dateStart, [BindRequired] DateTime dateEnd,
                                                                                                  int? cabinetLocationId, int? cabinetId, int? lockerTypeId,
                                                                                                  int pageSize, int currentPage, int? positionId = null)
        {
            var model = await _lockerService.GetAvailableLocker(cabinetLocationId, lockerTypeId, null, null, dateStart, dateEnd, false, cabinetId, currentPage, pageSize, positionId);

            return Ok(model);
        }

        #endregion

        #region Cabinet
        /// <summary>
        /// Get a list of cabinet 
        /// </summary>
        /// <returns> returns list cabinet </returns>
        /// <response code="200">CabinetModel list</response>
        [HttpGet("GetCabinet")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<CabinetModel>>> GetCabinet(int? cabinetid = null, int? companyId = null)
        {
            var model = await _cabinetService.GetCabinet(cabinetid, companyId);
            return Ok(model);
        }

        /// <summary>
        /// Add or update a cabinet. If the cabinetId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="cabinet">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveCabinet")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveCabinet([FromBody] CabinetModel cabinet)
        {
            var model = await _cabinetService.SaveCabinet(cabinet);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Activate or deactivate the record in database table. Does not delete the record
        /// <br/>
        /// </summary>
        /// <remarks> setting IsDelete = true will deactivate the record </remarks>
        /// <param name="cabinetId">CabinetId</param>
        /// <param name="isDeleted">IsDeleted</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPut("CabinetActivation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CabinetActivation([BindRequired] int cabinetId, [BindRequired] bool isDeleted)
        {
            var model = await _cabinetService.SetCabinetActivation(cabinetId, isDeleted);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Get a list of cabinet types 
        /// </summary>
        /// <returns> returns list cabinet types </returns>
        /// <response code="200">CabinetTypeEntity list</response>
        [HttpGet("GetCabinetTypes")]
        //[AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<CabinetTypeViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<CabinetTypeViewModel>>> GetCabinetTypes()
        {
            var model = await _cabinetService.GetCabinetTypes();
            return Ok(model);
        }

        /// <summary>
        /// Add or update a cabinet type. If the cabinetTypeId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="cabinetType">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveCabinetType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveCabinetType([FromBody] CabinetTypeViewModel cabinetType)
        {
            var model = await _cabinetService.SaveCabinetType(cabinetType);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        #endregion

        #region Pricing
        /// <summary>
        /// Get a list of pricing types 
        /// </summary>
        /// <returns> returns list pricing types </returns>
        /// <response code="200">PricingTypeModel list</response>
        [HttpGet("GetPricingTypes")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<PricingTypeModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<PricingTypeModel>>> GetPricingTypes()
        {
            var model = await _pricingService.GetPricingType();
            return Ok(model);
        }

        /// <summary>
        /// Add or update a pricing type. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="announcementType">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SavePricingType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SavePricingType([FromBody] PricingTypeModel announcementType)
        {
            var model = await _pricingService.SavePricingType(announcementType);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Delete pricing type. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeletePricingType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeletePricingType([FromQuery] int id)
        {
            var model = await _pricingService.DeletePricingType(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

       
        [HttpDelete("DeletePriceAndCharging")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeletePriceAndCharging([FromQuery] int id)
        {
            var model = await _pricingService.DeletePriceAndCharging(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Get a list of pricing Matrix Config
        /// </summary>
        /// <param name="PricingTypeId"></param>
        /// <returns> returns list pricing Matrix Config</returns>
        /// <response code="200">PricingMatrixConfigModel list</response>
        [HttpGet("GetPricingMatrixConfigs")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<PriceMatrixConfigModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<PriceMatrixConfigModel>>> GetPricingMatrixConfig([FromQuery] int? PricingTypeId = null,  int?SelectedId = null, short? IsActive=null)
        {
            var model = await _pricingService.GetPricingMatrixConfig(PricingTypeId,SelectedId,IsActive);
            return Ok(model);
        }

        [HttpGet("ActivateDeactivatePricingMatrixConfig")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<PriceMatrixConfigModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<PriceMatrixConfigModel>>> ActivateDeactivate([FromQuery] int Id, int? isActive=null)
        {
            var model = await _pricingService.ActivateDeactivate(Id,isActive);
            return Ok(model);
        }

        /// <summary>
        /// Add or update a price matrix config. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="pricingMatrixConfig">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SavePricingMatrixConfig")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SavePricingMatrixConfig([FromBody] PriceMatrixConfigModel pricingMatrixConfig)
        {
            var model = await _pricingService.SavePricingMatrixConfig(pricingMatrixConfig);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Delete pricing Matrix Config. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeactivatePricingMatrixConfig")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeletePriceMatrixConfig([FromQuery] int id)
        {
            var model = await _pricingService.DeletePriceMatrixConfig(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Get a list of Price and Chargings
        /// </summary>
        /// <returns> returns list Price and Charging</returns>
        /// <response code="200">PriceAndChargingModel list</response>
        [HttpGet("GetPriceAndChargings")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<PriceAndChargingModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<PriceAndChargingModel>>> GetPriceAndCharging()
        {
            var model = await _pricingService.GetPriceAndCharging();
            return Ok(model);
        }

        /// <summary>
        /// Add or update a PriceAndCharging. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="priceAndChargingModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SavePriceAndCharging")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SavePriceAndCharging([FromBody] PriceAndChargingModel priceAndChargingModel)
        {
            var model = await _pricingService.SavePriceAndCharging(priceAndChargingModel);
            if (model.MessageReturnNumber > 0)
                return base.Ok((object)model);
            else
                return base.Conflict((object)model);
        }

        /// <summary>
        /// Get a list of announcement type 
        /// </summary>
        /// <returns> returns list announcement type</returns>
        /// <response code="200">AnnouncementTypeModel list</response>
        [HttpGet("GetAnnouncementTypes")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<AnnouncementTypeModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<AnnouncementTypeModel>>> GetAnnouncementTypes()
        {
            var model = await announcementService.GetAnnouncementTypes();
            return Ok(model);
        }

        /// <summary>
        /// Add or update a announcement type. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="announcementType">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveAnnouncementType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveAnnouncementType([FromBody] AnnouncementTypeModel announcementType)
        {
            var model = await announcementService.SaveAnnouncementType(announcementType);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Delete announcement type. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteAnnouncementType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteAnnouncementType([FromQuery] int id)
        {
            var model = await announcementService.DeleteAnnouncementType(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Get a list of Promo Announcements
        /// </summary>
        /// <returns> returns list promo announcements</returns>
        /// <response code="200">PromoAnnouncementModel list</response>
        [HttpGet("GetPromoAnnouncements")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<PromoAnnouncementModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<PromoAnnouncementModel>>> GetPromoAnnouncements()
        {
            var model = await announcementService.GetPromoAnnouncements();
            return Ok(model);
        }

        /// <summary>
        /// Add or update a promo announcement. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="promoAnnouncement">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SavePromoAnnouncement")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SavePromoAnnouncement([FromBody] PromoAnnouncementModel promoAnnouncement)
        {
            var model = await announcementService.SavePromoAnnouncement(promoAnnouncement);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Delete promo announcement. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeletePromoAnnouncement")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeletePromoAnnouncement([FromQuery] int id)
        {
            var model = await announcementService.DeletePromoAnnouncement(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        #endregion

        #region Reports
        /// <summary>
        /// Get a list of Cleanliness Reports
        /// </summary>
        /// <returns> returns list Cleanliness Reports</returns>
        /// <response code="200">CleanlinessReportModel list</response>
        [HttpGet("GetCleanlinessReports")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(CleanlinessReportParentViewModel), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<CleanlinessReportParentViewModel>> GetCleanlinessReports([BindRequired] int Month, [BindRequired] int Year, int PageNumber, int PageSize, int? CompanyId, int? Status)
        {
            if (PageNumber <= 0)
            {
                PageNumber = 1;
            }
            if (PageSize <= 0)
            {
                PageSize = 10;
            }
            var model = await reportService.GetCleanlinessReport(Month, Year, PageNumber, PageSize, CompanyId, Status);


            return Ok(model);
        }

        /// <summary>
        /// Add or update a Cleanliness Report. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="cleanlinessReport">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveCleanlinessReport")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveCleanlinessReport([FromBody] CleanlinessReportModel cleanlinessReport)
        {
            var model = await reportService.SaveCleanlinessReport(cleanlinessReport);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Delete Cleanliness Report. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        /// 
        [HttpDelete("DeleteCleanlinessReport")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteCleanlinessReport([FromQuery] int id)
        {
            var model = await reportService.DeleteCleanlinessReport(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        #endregion

        #region Maintenance
        /// <summary>
        /// Get a list of Maintenance Reason Types
        /// </summary>
        /// <returns> returns list maintenance reason types </returns>
        /// <response code="200">MaintenanceReasonTypeModel list</response>
        [HttpGet("GetMaintenanceReasonTypes")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<PricingTypeModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<MaintenanceReasonTypeModel>>> GetMaintenanceReasonTypes()
        {
            var model = await maintenanceService.GetMaintenanceReasonType();
            return Ok(model);
        }

        /// <summary>
        /// Add or update a Maintenance Reason Type. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="maintenanceReasonTypeModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveMaintenanceReasonType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveMaintenanceReasonType([FromBody] MaintenanceReasonTypeModel maintenanceReasonTypeModel)
        {
            var model = await maintenanceService.SaveMaintenanceReasonType(maintenanceReasonTypeModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Delete MaintenanceReasonType. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteMaintenanceReasonType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteMaintenanceReasonType([FromQuery] int id)
        {
            var model = await maintenanceService.DeleteMaintenanceReasonType(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Get a list of maintenance inspection testing
        /// </summary>
        /// <param name="cabinetLocationId"></param>
        /// <param name="companyId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns> returns list maintenance inspection testing </returns>
        /// <response code="200">MaintenanceInspectionTestingViewModel list</response>
        [HttpGet("GetMaintenanceInspectionTestings")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<MaintenanceInspectionTestingViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<MaintenanceInspectionTestingViewModel>>> GetMaintenanceInspectionTesting(DateTime? fromDate, DateTime? toDate, int? companyId, int? cabinetLocationId)
        {
            var model = await maintenanceService.GetMaintenanceInspectionTesting(fromDate, toDate, companyId, cabinetLocationId);
            return Ok(model);
        }

        /// <summary>
        /// Add or update a maintenance inspection testing. If the id is not zero it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="maintenanceInspectionTesting">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveMaintenanceInspectionTesting")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveMaintenanceInspectionTesting([FromBody] MaintenanceInspectionTestingModel maintenanceInspectionTesting)
        {
            var model = await maintenanceService.SaveMaintenanceInspectionTesting(maintenanceInspectionTesting);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Delete maintenance inspection testing. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteMaintenanceInspectionTesting")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteMaintenanceInspectionTesting([FromQuery] int id)
        {
            var model = await maintenanceService.DeleteMaintenanceInspectionTesting(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        #endregion

        #region Cabinet Location

        /// <summary>
        /// Get a list of cabinet location
        /// </summary>
        /// <returns> returns list  cabinet </returns>
        /// <response code="200">CabinetLocationViewModel list</response>
        [HttpGet("GetCabinetLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<CabinetLocationViewModel>>> GetCabinetLocation()
        {
            var model = await _cabinetService.GetCabinetLocation();
            return Ok(model);
        }

        [HttpGet("GetAvailableCabinetLocationByCompany")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<CabinetLocationViewModel>>> GetAvailableCabinetLocationByCompany(int companyId)
        {
            var model = await _cabinetService.GetCabinetLocationByCompany(companyId);
            return Ok(model);
        }

        /// <summary>
        /// Get a list of  cabinet and it's location
        /// </summary>
        /// <returns> returns list  cabinet </returns>
        /// <response code="200">CabinetViewModel list</response>
        [HttpGet("GetCabinetWithLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<CabinetViewModel>>> GetCabinetWithLocation(bool IsMapRegion)
        {
            var model = await _cabinetService.GetCabinetWithLocation(IsMapRegion);
            return Ok(model);
        }

        /// <summary>
        /// Add or update a cabinet location. If the cabinetLocationId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="cabinetLocation">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveCabinetLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveCabinetLocation([FromBody] CabinetLocationModel cabinetLocation)
        {
            var model = await _cabinetService.SaveCabinetLocation(cabinetLocation);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Activate or deactivate the record in database table. Does not delete the record
        /// <br/>
        /// </summary>
        /// <remarks> setting IsDelete = true will deactivate the record </remarks>
        /// <param name="cabinetLocationId">CabinetId</param>
        /// <param name="isDeleted">IsDeleted</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPut("CabinetLocationActivation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CabinetLocationActivation([BindRequired] int cabinetLocationId, [BindRequired] bool isDeleted)
        {
            var model = await _cabinetService.SetCabinetLocationActivation(cabinetLocationId, isDeleted);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        #endregion

        #region Locker Detail
        /// <summary>
        /// Add or update a locker. If the lockerDetailId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="lockerDetail">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        ///        /// <remarks>
        ///     For parameters<br/><br/>
        ///     OTP<br/>
        ///     Locker Position in Cabinet<br/>
        ///     1 - Low <br/>
        ///     2 - Middle <br/>
        ///     3 - High <br/>
        /// </remarks>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveLockerDetail")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveLockerDetail([FromBody] LockerDetailModel lockerDetail)
        {
            var model = await _lockerService.SaveLocker(lockerDetail);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Activate or deactivate the record in database table. Does not delete the record
        /// <br/>
        /// </summary>
        /// <remarks> setting IsDelete = true will deactivate the record </remarks>
        /// <param name="lockerDetailId">CabinetId</param>
        /// <param name="isDeleted">IsDeleted</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPut("LockerDetailActivation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> LockerDetailActivation([BindRequired] int lockerDetailId, [BindRequired] bool isDeleted)
        {
            var model = await _lockerService.SetLockerDetailActivation(lockerDetailId, isDeleted);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }


        /// <summary>
        /// Get a list of locker zones 
        /// </summary>
        /// <returns> returns list lcoker zones </returns>
        /// <response code="200">CabinetTypeEntity list</response>
        [HttpGet("GetLockerZones")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<LockerZoneModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<LockerZoneModel>>> GetLockerZones()
        {
            var model = await _lockerService.GetLockerZones();
            return Ok(model);
        }
        /// <summary>
        /// Get a list of Reassigned Booking Locker Model
        /// </summary>
        /// <returns> returns list Reassigned Booking Locker </returns>
        /// <response code="200">ReassignedBookingLockerModel list</response>
        [HttpGet("GetReassignedBookingLockerHistory")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<ReassignedBookingLockerModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<ReassignedBookingLockerModel>>> GetReassignedBookingLockerHistory(int? lockerDetailId = null, int? lockerTransactionsId = null, int? adminUserId = null, int? companyUserId = null, int? companyId = null)
        {
            var model = await _lockerService.GetReassignedBookingLockerHistoryForAdmin(lockerDetailId, lockerTransactionsId, adminUserId, companyUserId, companyId);
            return Ok(model);
        }

        /// <summary>
        /// update Booking's Locker Detail Id.
        /// <br/>
        /// </summary>
        /// <param name="bookingLockerDetailModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("ReassignBookingLocker")]
       // [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> UpdateBookingLockerDetail([FromBody] BookingLockerDetailModel bookingLockerDetailModel)
        {
            var model = await _lockerService.ReassignBookingForAdmin(bookingLockerDetailModel, User.Identity.Name);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        /// <summary>
        /// Add or update a locker zone. If the positionId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="lockerZone">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveLockerZone")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveLockerZone([FromBody] LockerZoneModel lockerZone)
        {
            var model = await _lockerService.SaveLockerZone(lockerZone);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        [HttpDelete("DeleteLockerZone")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteLockerZone([FromQuery] int id)
        {
            var model = await _lockerService.DeleteLockerZone(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        /// <summary>
        /// Get a list of active Locker Status
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="cabinetId"></param>
        /// <returns> returns list Locker Detail Status</returns>
        /// <response code="200">LockerDetailStatusModel list</response>
        [HttpGet("GetLockerStatus")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]

        public async Task<ActionResult<List<LockerDetailStatusModel>>> GetActiveLockerStatus(int? companyId = null, int? cabinetId = null)
        {
            var model = await _lockerService.GetActiveLockerStatus(null, companyId, cabinetId: cabinetId);
            return Ok(model);
        }

        #endregion

        #region Parent Company

        /// <summary>
        /// Get all the parent company
        /// <br/>
        /// </summary>
        /// <remarks>this is using an OR operator on the query</remarks>
        /// <returns> returns list of company, </returns>
        /// <response code="200">returns list of company</response>
        [HttpGet("GetParentCompany")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<ParentCompanyViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<ParentCompanyViewModel>>> GetParentCompany(string parentCompanyKeyId, string parentCompanyName)
        {
            var model = await _parentCompanyService.GetParentCompany(false, parentCompanyKeyId, parentCompanyName);
            return Ok(model);
        }


        /// <summary>
        /// Add or update a company. If the companyKeyId is null it will perform an update else it will insert.
        /// <br/>
        /// </summary>
        /// <param name="parentCompanyPostModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveParentCompany")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveParentCompany([FromBody] ParentCompanyPostModel parentCompanyPostModel)
        {
            var model = await _parentCompanyService.SetParentCompany(parentCompanyPostModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }



        #endregion

        #region Franchisee Company

        /// <summary>
        /// Add or update a company. If the companyKeyId is null it will perform an update else it will insert.
        /// <br/>
        /// </summary>
        /// <param name="companyModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveCompany")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveCompany([FromBody] CompanyModel companyModel)
        {
            var model = await _companyService.SaveCompany(companyModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Get all the company
        /// <br/>
        /// </summary>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">returns list of company</response>
        [HttpPost("GetCompany")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<CompanyViewModel>>> GetCompany()
        {
            var model = await _companyService.GetCompanyList();
            return Ok(model);

        }
        /// <summary>
        /// Send Email to multiple receipent.
        /// <br/>
        /// </summary>
        /// <param name="emailModel">Email Model. Use ; for multiple email address for To and CC</param>
        /// /// <param name="companyId">if this email is for specific company</param>
        /// <returns> returns ok if success</returns>
        /// <response code="200"></response>
        [HttpPost("SendEmail")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ActionResult), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> SendEmail(EmailModel emailModel, [FromQuery] int? companyId = null)
        {
            if (await notificationService.SendSmtpEmailAsync(emailModel, companyId))
                return Ok();
            else return Conflict();

        }

        /// <summary>
        /// Send Email to multiple receipent.
        /// <br/>
        /// </summary>
        /// <param name="emailModel">Email Model</param> 
        /// <returns> returns ok if success</returns>
        /// <response code="200"></response>

        [HttpPost("SaveEmailMessage")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveEmailMessage([FromBody] EmailModel emailModel)
        {
            var model = await _emailMessageService.Save(emailModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Add or update a Company Cabinet. If the positionId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="companyCabinetModel"></param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("SaveCompanyCabinet")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveCompanyCabinet([FromBody] CompanyCabinetModel companyCabinetModel)
        {
            var model = await _companyService.SaveCompanyCabinet(companyCabinetModel);
            if (model.MessageReturnNumber > 0)
                return base.Ok((object)model);
            else
            {
                return base.Conflict((object)model);
            }
        }
        [HttpPost("AssignCompanyCabinetToLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> AssignCompanyCabinetToLocation([FromBody] AssignCompanyCabinetModel model)
        {
            var response = await _companyService.AssignCompanyCabinetToLocation(model);
            if (response.MessageReturnNumber > 0)
                return Ok(response);
            else
            {
                return Conflict(response);
            }
        }
        [HttpPost("UnAssignCompanyCabinetToLocation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> UnAssignCompanyCabinetToLocation([FromBody] AssignCompanyCabinetModel model)
        {
            var response = await _companyService.UnAssignCompanyCabinetToLocation(model);
            if (response.MessageReturnNumber > 0)
                return Ok(response);
            else
            {
                return Conflict(response);
            }
        }
        [HttpGet("GetUnAssignedCompany")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<UnassignCompanyModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<UnassignCompanyModel>>> GetUnAssignedCompanies(int? companyId = null)
        {
            var model = await _companyService.GetUnAssignedCompanies(companyId);
            return Ok(model);
        }
        [HttpGet("GetUnAssignedCompanyCabinet")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<UnassignedCompanyCabinetModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<UnassignedCompanyCabinetModel>>> GetUnAssignedCompanyCabinets(int? cabinetId = null)
        {
            var model = await _companyService.GetUnAssignedCompanyCabinets(cabinetId);
            return Ok(model);
        }

        #endregion

        #region Roles
        /// <summary>
        /// Add or update a roles type. If the roleId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="role">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreateRole")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateRole([FromBody] RoleModel role)
        {
            var model = await _roleService.Save(role);
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
        [HttpGet("GetRoles")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<RoleViewModel>>> GetRoles()
        {
            var model = await _roleService.GetAll();
            return Ok(model);
        }

        /// <summary>
        /// Get role by id
        /// </summary>
        /// <returns> role by id </returns>
        /// <response code="200">RoleViewModel list</response>
        [HttpGet("GetRoleById")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<RoleViewModel>>> GetRoleById(int Id)
        {
            var model = await _roleService.GetById(Id);
            return Ok(model);
        }
        /// <summary>
        /// Delete role. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteRole")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteRole([FromQuery] int id)
        {
            var model = await _roleService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        #endregion

        #region UserRole

        /// <summary>
        /// Add or update a Useroles type. If the roleId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="userRole">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreateUserRole")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateUserRole([FromBody] UserRoleModel userRole)
        {
            var model = await _userRoleService.Save(userRole);
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
        [HttpGet("GetUserRolesByUserId")]     
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<UserRoleViewModel>>> GetUserRolesByUserId(int UserId)
        {
            var model = await _userRoleService.GetAll(UserId);
            return Ok(model);
        }

        /// <summary>
        /// Get role by id
        /// </summary>
        /// <returns> UserRole by id </returns>
        /// <response code="200">RoleViewModel list</response>
        [HttpGet("GetUserRoleById")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<UserRoleViewModel>>> GetUserRoleById(int Id)
        {
            var model = await _userRoleService.GetById(Id);
            return Ok(model);
        }



        /// <summary>
        /// Delete User Role. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteUserRole")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteUserRole([FromQuery] int id)
        {
            var model = await _userRoleService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        #endregion

        #region Permission
        /// <summary>
        /// Add or update a roles type. If the roleId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="permissionModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreatePermission")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateRole([FromBody] PermissionModel permissionModel)
        {
            var model = await _permissionService.Save(permissionModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Get a list of permissions
        /// </summary>
        /// <returns> returns list roles </returns>
        /// <response code="200">RoleViewModel list</response>  
        [HttpGet("GetPermissions")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<PermissionViewModel>>> GetPermissions()
        {
            var model = await _permissionService.GetAll();
            return Ok(model);
        }

        /// <summary>
        /// Get permission  by id
        /// </summary>
        /// <returns> role by id </returns>
        /// <response code="200">RoleViewModel list</response>
        [HttpGet("GetPermissionById")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<PermissionViewModel>>> GetPermissionById(int Id)
        {
            var model = await _permissionService.GetById(Id);
            return Ok(model);
        }

        /// <summary>
        /// Delete role. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeletePermission")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeletePermission([FromQuery] int id)
        {
            var model = await _permissionService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        #endregion

        #region RolePermission


        /// <summary>
        /// Add or update a RolePermission type. If the roleId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="rolePermissionModel">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreateRolePermission")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateRolePermission([FromBody] RolePermissionModel rolePermissionModel)
        {
            var model = await _rolePermissionService.Save(rolePermissionModel);
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
        /// <returns> returns list rolePermission </returns>
        /// <response code="200">RolePermissionDetailModel list</response>
        [HttpGet("GetRolePermissionsByRoleId")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<RolePermissionDetailModel>>> GetRolePermissionsByRoleId(int? RoleId = null)
        {
            var model = await _rolePermissionService.GetRolePermissions(RoleId);
            return Ok(model);
        }

        /// <summary>
        /// Get role by id
        /// </summary>
        /// <returns> RolePermission by id </returns>
        /// <response code="200">RoleViewModel list</response>
        [HttpGet("GeRolePermissionById")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<RolePermissionViewModel>>> GetRolePermissionById(int Id)
        {
            var model = await _rolePermissionService.GetById(Id);
            return Ok(model);
        }



        /// <summary>
        /// Delete Role Permision. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteRolePermission")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteRolePermission([FromQuery] int id)
        {
            var model = await _rolePermissionService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }


        #endregion

        #region Frenchise Feedback
        /// <summary>
        /// Add or update a Frenchise Feedback. If the roleId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="franchiseFeedbackQuestion">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreateFeedbackQuestions")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateFeedbackQuestions([FromBody] FranchiseFeedbackQuestionViewModel franchiseFeedbackQuestion)
        {
            var model = await _franchiseFeedbackService.Save(franchiseFeedbackQuestion);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Get a list of Frenchise Feedback questions
        /// </summary>
        /// <returns> returns list roles </returns>
        /// <response code="200">FranchiseFeedbackQuestionModel list</response>  
        [HttpGet("GetFeedbackQuestions")]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<FranchiseFeedbackQuestionModel>>> GetFeedbackQuestions()
        {
            var model = await _franchiseFeedbackService.GetAllQuestions();
            return Ok(model);
        }

        [HttpGet("GetFeedbackQuestionsByType")]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<FranchiseFeedbackQuestionModel>>> GetFeedbackQuestionsByType([FromQuery] int? type)
        {
            var surveyType = GlobalEnums.FranchiseFeedbackQuestionType.DropOff;
            if (type == 2)
                surveyType = GlobalEnums.FranchiseFeedbackQuestionType.Pickup;
            else if (type == 3)
                surveyType = GlobalEnums.FranchiseFeedbackQuestionType.Franchise;

            var model = await _franchiseFeedbackService.GetAllQuestionsByType(surveyType);
            return Ok(model);
        }

        /// <summary>
        /// Delete question. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteFeedbackQuestion")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteFeedbackQuestion([FromQuery] int id)
        {
            var model = await _franchiseFeedbackService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        #endregion

        #region Frenchise Feedback Answer
        /// <summary>
        /// Add or update a roles type. If the roleId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="franchiseFeedbackAnswer)">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreateFeedbackAnswer")]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateFeedbackAnswer([FromBody] FranchiseFeedbackAnswerViewModel franchiseFeedbackAnswer)
        {
            var model = await _franchiseFeedbackService.SaveAnswer(franchiseFeedbackAnswer);
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
        /// <returns> returns list FeedbackAnswer </returns>
        /// <response code="200">FranchiseFeedbackAnswerModel list</response>  
        [HttpGet("GetFeedbackAnswer")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<FranchiseFeedbackAnswerModel>>> GetFeedbackAnswer()
        {


            var model = await _franchiseFeedbackService.GetAllAnswers();
            return Ok(model);
        }

        [HttpGet("GetFeedbackAnswersByType")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<FranchiseFeedbackAnswerModel>>> GetFeedbackAnswerByType([FromQuery] int? type)
        {
            var surveyType = GlobalEnums.FranchiseFeedbackQuestionType.DropOff;
            if (type == 2)
                surveyType = GlobalEnums.FranchiseFeedbackQuestionType.Pickup;
            else if (type == 3)
                surveyType = GlobalEnums.FranchiseFeedbackQuestionType.Franchise;


            var model = await _franchiseFeedbackService.GetAllAnswerByType(surveyType);
            return Ok(model);
        }


        /// <summary>
        /// Delete FeedbackAnswer. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteFeedbackAnswer")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteFeedbackAnswer([FromQuery] int id)
        {
            var model = await _franchiseFeedbackService.DeleteAnswer(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        #endregion

        #region Messsage Logs
        /// <summary>
        /// Get a list of message logs 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns> returns list of message log</returns>
        /// <response code="200">MessageLogModel list</response>
        [HttpGet("GetMessageLogs")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<MessageLogModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<MessageLogModel>>> GetMessageLogs([FromQuery] int? companyId = null, [FromQuery] int? currentPage = null, [FromQuery] int? pageSize = null)
        {
            var model = await notificationService.GetMessageLogs(companyId, currentPage, pageSize);
            return Ok(model);
        }
        #endregion

        #region Bookings
        /// <summary>   
        /// Get a list of available Locker for booking
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// /// <param name="companyId"></param>
        /// <param name="currentPage"></param>
        ///  /// <param name="pageSize"></param>
        ///  <param name="bookingStatus"></param>
        /// <returns> returns list available locker based on the parameter supplied</returns>
        /// <response code="200">BookingTransactionsViewModel list</response>
        [HttpGet("GetBookingTransactions")]
        //[AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<BookingTransactionsViewModel>> GetBookingTransactions(DateTime? startDate, DateTime?
                                                                                endDate, int? companyId = null, int? currentPage = null, int? pageSize = null, BookingTransactionStatus? bookingStatus = null)
        {
            var model = await _lockerService.GetBookingTransactions(startDate, endDate, companyId, currentPage, pageSize, bookingStatus);
            return Ok(model);
        }

        [HttpGet("GetLockerReassignments")]
        //[AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<BookingTransactionsViewModel>> GetLockerReassignments(DateTime? startDate, DateTime?
                                                                               endDate, int? companyId = null, int? currentPage = null, int? pageSize = null, BookingTransactionStatus? bookingStatus = null)
        {
            var model = await _lockerService.GetBookingTransactions(startDate, endDate, companyId, currentPage, pageSize, bookingStatus);

            var modelList = new List<BookingTransactionsViewModel>();
            foreach (var l in model)
            {
                if (!(DateTime.Now > l.StoragePeriodEnd && l.BookingStatus == 2))
                    modelList.Add(l);
            }

            return Ok(modelList);
        }
        /// <summary>
        /// Send Booking Receipt Email
        /// <br/>
        /// </summary>
        /// <param name="bookingReceiptModel"></param>
        /// <returns> returns true if successfully send else false</returns>
        [HttpPost("SendBookingReceiptEmail")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<bool>> SendBookingReceiptEmail([FromBody] BookingReceiptModel bookingReceiptModel)
        {
            return await _lockerService.SendBookingReceiptEmail(bookingReceiptModel);

        }


        /// <summary>
        /// Update the booking transaction,
        /// </summary>
        /// <param name="postUpdateBookingStatusModel"></param>
        /// <remarks> 
        ///     <br/> Will only update if the current booking transaction status For-Dropoff or ForPickup status.
        ///     <br/> Can only update the booking status to Confiscated
        /// </remarks>
        /// <returns> returns ResponseValidityModel </returns>
        /// <response code="200">
        ///   returns information <br/>
        ///   501 - Record was updated <br/>
        ///   503 - No records found  (booking transaction id not exits)<br/>
        /// </response>
        /// <response code="400">
        ///   return error number for the ff: <br/>
        ///   -600 - Unexpected system error. Please contact the admin. (this is unable to save, update or delete in Database) <br/>
        ///   -603 - No record was added/updated/deleted <br/>
        ///   -605 - booking status not existing <br/>
        ///   -22 - You can only update the Booking Status to Confiscated <br/>
        ///   -21 - The Locker you're trying to subscribe was already close or completed status. Please try to subcribe this locker on a  different date period <br/>
        ///   -23 - The booking transaction status you're trying to update were either completed, confiscated or expired <br/>
        /// </response>
        [HttpPost("UpdateBookingTransaction")]

        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ResponseValidityModel>> UpdateBookingTransaction([FromBody] PostUpdateBookingStatusModel postUpdateBookingStatusModel)
        {
            var model = await _lockerService.UpdateLockerBookingStatus(postUpdateBookingStatusModel);

            if (model.MessageReturnNumber >= 0)
            {
                return Ok(model);
            }
            else
            {
                return BadRequest(model);
            }

        }
        [HttpGet("GetUserBookings")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<LockerBookingViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<LockerBookingViewModel>>> GetUserBookings(string userKeyId, BookingTransactionStatus? bookingStatus = null, DateTime? fromDate = null,
            DateTime? toDate = null, int? currentPage = null, int? pageSize = null, bool isActiveOnly = false)
        {
            var model = await _lockerService.GetUserBookings(userKeyId, bookingStatus, fromDate: fromDate, toDate: toDate, currentPage, pageSize, isActiveOnly);
            return Ok(model);
        }
        #endregion

        #region Application Settings
        /// <summary>
        /// Get a application setting set by admin of the company
        /// </summary>
        /// <remarks>
        /// <br/>MaintainanceReportReminderHour - save in military time format
        /// <br/>MaintainanceOverdueReportReminderHour - save in military time format
        /// </remarks>
        /// <response code="200">Application settings Model</response>
        [HttpGet("GetApplicationSettings")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<ApplicationSettingModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<ApplicationSettingModel>> GetApplicationSettings(short applicationSettingId = 1)
        {
            var model = await _applicationSettingService.GetApplicationSetting(applicationSettingId);
            return Ok(model);
        }
        [HttpPost("SaveApplicationSetting")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SaveApplicationSetting([FromBody] ApplicationSettingModel applicationSettingModel)
        {
            var model = await _applicationSettingService.Save(applicationSettingModel);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        #endregion

        #region Application Message

        /// <summary>
        /// Get all application Messages set by admin of the company
        /// </summary>
        /// <remarks>
        /// <br/>MaintainanceReportReminderHour - save in military time format
        /// <br/>MaintainanceOverdueReportReminderHour - save in military time format
        /// </remarks>
        /// <response code="200">Application Message Model</response>
        [HttpGet("GetAllApplicationMessage")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<ApplicationMessageModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<ApplicationMessageModel>>> GetAllApplicationMessage()
        {
            var model = await _appMessageService.GetAll();
            return Ok(model);
        }



        /// <summary>
        /// Get a application Message set by admin of the company
        /// </summary>
        /// <remarks>
        /// <br/>MaintainanceReportReminderHour - save in military time format
        /// <br/>MaintainanceOverdueReportReminderHour - save in military time format
        /// </remarks>
        /// <response code="200">Application Message Model</response>
        [HttpGet("GetApplicationMessage")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(ApplicationMessageModel), (int)HttpStatusCode.Accepted)]
        [AllowAnonymous]
        public async Task<ActionResult<ApplicationMessageModel>> GetApplicationMessageModel(int Id)
        {
            var model = await _appMessageService.GetApplicationMessageById(Id);
            return Ok(model);
        }

        /// <summary>
        /// Create An Application Message.
        /// <br/>
        /// </summary>
        /// <param name="appMessage">Request's payload</param>
        /// <returns> returns if validity model if successfully Created</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreateApplicationMessage")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateApplicationMessage([FromBody] ApplicationMessageModel appMessage)
        {
            var model = await _appMessageService.Create(appMessage);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Update An Application Message.
        /// <br/>
        /// </summary>
        /// <param name="appMessage">Request's payload</param>
        /// <returns> returns if validity model if successfully Created</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("UpdateApplicationMessage")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> UpdateApplicationMessage([FromBody] ApplicationMessageModel appMessage)
        {
            var model = await _appMessageService.Update(appMessage);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }


        /// <summary>
        /// DeleteApplicationMessage. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteApplicationMessage")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> DeleteApplicationMessage([FromQuery] int id)
        {
            var model = await _appMessageService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }
        #endregion

        #region PromoAndDiscounts
        /// <summary>
        /// Add or update a PromoAndDiscounts. If the PromoAndDiscountsId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="role">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreatePromoAndDiscounts")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreatePromoAndDiscounts([FromBody] PromoAndDiscountsModel PromoAndDiscounts)
        {
            var model = await _promoAndDiscountsService.Save(PromoAndDiscounts);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Get a list of PromoAndDiscounts
        /// </summary>
        /// <returns> returns list PromoAndDiscounts </returns>
        /// <response code="200">RoleViewModel list</response>  
        [HttpGet("GetPromoAndDiscounts")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public ActionResult<List<PromoAndDiscountsModel>> GetPromoAndDiscounts()
        {
            var model =  _promoAndDiscountsService.GetAll();
            return Ok(model);
        }

        /// <summary>
        /// Get role by id
        /// </summary>
        /// <returns> role by id </returns>
        /// <response code="200">RoleViewModel list</response>
        [HttpGet("GetPromoAndDiscountsById")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<PromoAndDiscountsViewModel>>> GetPromoAndDiscountsById(int Id)
        {
            var model = await _promoAndDiscountsService.GetById(Id);
            return Ok(model);
        }
        /// <summary>
        /// Delete PromoAndDiscounts. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeletePromoAndDiscounts")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> PromoAndDiscounts([FromQuery] int id)
        {
            var model = await _promoAndDiscountsService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        #endregion

        #region Ads
        /// <summary>
        /// Add or update an Ads. If the AdsId is not zero or null it will perform an update.
        /// <br/>
        /// </summary>
        /// <param name="role">Request's payload</param>
        /// <returns> returns if validity model if successfully save</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpPost("CreateAds")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CreateAds([FromBody] AdsModel Ads)
        {
            var model = await _adsService.Save(Ads);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Get a list of Ads
        /// </summary>
        /// <returns> returns list Ads </returns>
        /// <response code="200">RoleViewModel list</response>  
        [HttpGet("GetAds")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public ActionResult<List<AdsModel>> GetAds()
        {
            var model = _adsService.GetAll();
            return Ok(model);
        }

        /// <summary>
        /// Get Ads by id
        /// </summary>
        /// <returns> role by id </returns>
        /// <response code="200">AdsModel list</response>
        [HttpGet("GetAdsById")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<AdsViewModel>>> GetAdsById(int Id)
        {
            var model = await _adsService.GetById(Id);
            return Ok(model);
        }
        /// <summary>
        /// Delete Ads. Id is required
        /// <br/>
        /// </summary>
        /// <param name="id">Request's payload</param>
        /// <returns> returns if validity model if successfully delete</returns>
        /// <response code="200">ResponseValidityModel either success or failed</response>
        [HttpDelete("DeleteAds")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> Ads([FromQuery] int id)
        {
            var model = await _adsService.Delete(id);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
                return Conflict(model);
        }

        #endregion


        [HttpPost("SendFCMNotification")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(FCMResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<FCMResponseModel>> SendFCMNotification([FromBody] FCMNotificationRequest request)
        {
            request.clickAction = "OPEN_APP";
            return Ok(await notificationService.SendFCMNotificationAsync(request));

        }


        /// <summary>
        /// Get a list of nlogs Feedback questions
        /// </summary>
        /// <returns> returns list roles </returns>
        /// <response code="200">FranchiseFeedbackQuestionModel list</response>  
        [HttpGet("GetLogs")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<LogsParentViewModel>>> GetLogs(DateTime? dateFrom, DateTime? dateTo, string logType, string search, int? PageNumber, int? PageSize)
        {
            int iPageNumber = PageNumber == null ? 1 : PageNumber.Value;
            int iPageSize = PageSize == null ? 10 : PageSize.Value;
            var model = await _logService.GetLogs(dateFrom, dateTo, logType, search, iPageNumber, iPageSize);
            return Ok(model);
        }
    }
}
