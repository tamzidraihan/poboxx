using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Cabinet;
using SmartBox.Business.Services.Service.Locker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BookingController : ControllerBase
    {
        private readonly ICabinetService _cabinetService;
        private readonly ILockerService _lockerService;
        public BookingController(ICabinetService cabinetService, ILockerService lockerService)
        {
            _cabinetService = cabinetService;
            _lockerService = lockerService;
        }

        /// <summary>
        /// Save user's booking using mobile booking, user is required to be log-in.
        /// <br/>
        /// Upon saving booking, when there's an available the system will automatically select a locker or else return an error message.
        /// </summary>
        /// <param name="lockerMobileBookingModel">Request's payload</param>
        /// <returns> returns OTPModel with drop-off PIN code</returns>
        /// <remarks> all fields are required, except the UserKey Id</remarks>
        /// <response code="200">Booking successfully save</response>
        [HttpPost("SaveBooking")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(OTPModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<OTPModel>> SaveBooking([FromBody] List<LockerMobileBookingModel> lockerMobileBookingModel)
        {
            var model = await _lockerService.SaveBookingLocker(lockerMobileBookingModel, User.Identity.Name);
            if (model.Where(e => e.ValidityModel.MessageReturnNumber > 0).Count() > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        /// <summary>
        /// Save user's booking using maual booking.
        /// <br/>
        /// Upon saving booking, when there's an available the system will automatically select a locker or else return an error message.
        /// </summary>
        /// <param name="lockerManualBooking">Request's payload</param>
        /// <returns> returns OTPModel with drop-off PIN code</returns>
        /// <remarks> all fields are required</remarks>
        /// <response code="200">Booking successfully save</response>
        [HttpPost("SaveManualBooking")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(OTPModel), (int)HttpStatusCode.Conflict)]

        public async Task<ActionResult<List<OTPModel>>> SaveManualBooking([FromBody] List<LockerManualBookingModel> lockerManualBooking)
        {
            var model = await _lockerService.SaveLockerBooking(lockerManualBooking);
            if (model.Where(e => e.ValidityModel.MessageReturnNumber > 0).Count() > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        /// <summary>
        /// Validate the PIN for drop-off and pick-up PIN.
        /// <br/>
        /// </summary>
        /// <param name="OTP">OTP of it's respected booking status</param>
        /// <param name="cabinetId"></param>
        /// <param name="companyId"></param>
        /// <param name="bookingStatus">The booking status</param>
        /// <returns> returns if the PIN is valid</returns>
        /// <remarks>
        ///     For parameters<br/><br/>
        ///     OTP<br/>
        ///     Booking Status<br/>
        ///     1 - For Drop-off <br/>
        ///     2 - For Pick-up <br/>
        /// </remarks>
        /// <response code="200">OTP is valid</response>
        [HttpGet("ValidateOTP")]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(OTPModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<OTPModel>> ValidateOTP(string OTP, int bookingStatus, int cabinetId, int? companyId = null)
        {
            var model = await _lockerService.ValidateOTP(OTP, bookingStatus, cabinetId, companyId);
            if (model?.ValidityModel?.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }


        /// <summary>
        /// By providing the lockertransactionId, the system will automatically update the status and generate new PIN code.
        /// <br/>
        /// </summary>
        /// <param name="lockerTransactionId">Request's parameter</param>

        /// <returns> returns generated PIN or transaction completed</returns>
        /// <remarks>OTP</remarks>
        /// <response code="200">Will return PIN code for pick-up or a message if transaction was alreaady completed</response>
        [HttpPut("GenerateOTP")]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(OTPModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<OTPModel>> GenerateOTP(int lockerTransactionId/*, int cabinetId, int? companyId = null*/)
        {
            var model = await _lockerService.GenerateOTP(lockerTransactionId/*, cabinetId, companyId*/);
            if (model?.ValidityModel?.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        [HttpPut("GenerateOTPForSubscriber")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(OTPModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<OTPModel>> GenerateOTPForSubscriber(int lockerTransactionId/*, int cabinetId, int? companyId = null*/)
        {
            var model = await _lockerService.GenerateOTP(lockerTransactionId, /*cabinetId, companyId,*/ User.Identity.Name);
            if (model.ValidityModel.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        /// <summary>
        /// Get Dropoff history
        /// <br/>
        /// </summary>
        /// <param name="userKeyId">Request's parameter</param>
        /// <returns> returns booking dropoff</returns>
        /// <remarks>userKeyId</remarks>
        /// <response code="200">Will return list of booking transaction where dropoff date is not null</response>
        [HttpGet("GetDropOffHistory")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<LockerBookingHistoryModel>), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<List<LockerBookingHistoryModel>>> GetDropOffHistory(string userKeyId)
        {
            var model = await _lockerService.GetDropOffHistory(userKeyId);
            return Ok(model);


        }
        /// <summary>
        /// Get Pickup history
        /// <br/>
        /// </summary>
        /// <param name="userKeyId">Request's parameter</param>
        /// <returns> returns booking pickup</returns>
        /// <remarks>userKeyId</remarks>
        /// <response code="200">Will return list of booking transaction where pickup date is not null</response>
        [HttpGet("GetPickupHistory")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<LockerBookingHistoryModel>), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<List<LockerBookingHistoryModel>>> GetPickupHistory(string userKeyId)
        {
            var model = await _lockerService.GetPickupHistory(userKeyId);
            return Ok(model);


        }
        [HttpPost("ExtendLockerBooking")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> ExtendLockerBooking([FromBody] ExtendLockerBookingModel extendLockerBooking)
        {
            var model = await _lockerService.ExtendLockerBooking(extendLockerBooking, User.Identity.Name);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        [HttpPost("Cancel")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> CancelBooking([FromBody] CancelBookingModel cancelBookingModel)
        {
            var model = await _lockerService.CancelBooking(cancelBookingModel, User.Identity.Name);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }
        [HttpPost("ValidateLockerBookingExtension")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public ActionResult<ResponseValidityModel> ValidateLockerBookingExtension([FromBody] ExtendLockerBookingModel extendLockerBooking)
        {
            ResponseValidityModel model = null;
            if (extendLockerBooking.StartStorageDateTime > DateTime.Now)
                model = _lockerService.ValidateLockerBooking(extendLockerBooking, User.Identity.Name);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        [HttpGet("GetLockerBookingByTransactionId")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<LockerBookingViewModel>> GetLockerBookingByTransactionId([BindRequired] int lockerTransactionId)
        {
            if (lockerTransactionId <= 0)
                return BadRequest("Please input a valid LockerTransactionId");

            var model = await _lockerService.GetLockerBookingByTransactionId(lockerTransactionId);
            return Ok(model);
        }
        [Authorize]
        [HttpGet("GetUserBookings")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(List<LockerBookingViewModel>), (int)HttpStatusCode.Accepted)]
        public async Task<ActionResult<List<LockerBookingViewModel>>> GetUserBookings(BookingTransactionStatus? bookingStatus = null, DateTime? fromDate = null,
            DateTime? toDate = null, int? currentPage = null, int? pageSize = null, bool activeOnly = false)
        {
            var model = await _lockerService.GetUserBookings(User.Identity.Name, bookingStatus, fromDate: fromDate, toDate: toDate, currentPage, pageSize, activeOnly);
            return Ok(model);
        }


        [HttpGet("ValidateSelectedLocker")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
     
        public async Task<ActionResult<bool>> ValidateAvailableLocker(int cabinetLocationId, int lockerTypeId, int positionId,
       int lockerDetailId)
        {
            var model = await _lockerService.ValidateAvailableLocker(cabinetLocationId, lockerTypeId, positionId,
                 lockerDetailId);

            return Ok(model);
        }

        [HttpPost("SelectLockerForBooking")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> SelectLockerForBooking([FromBody] SelectedLockerModel lockerModel)
        {
            ResponseValidityModel model = null;
 
                model =await _lockerService.SelectLockerForBooking(lockerModel.CabinetLocationId, lockerModel.LockerTypeId,
                    lockerModel.PositionId, lockerModel.storageStartDate, lockerModel.StorageEndDate, lockerModel.UserKeyId,
                    lockerModel.LockerDetailId);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }

        [HttpPost("ClearLockerForBooking")]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ProducesResponseType(typeof(ResponseValidityModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ResponseValidityModel>> ClearLockerForBooking([FromBody] SelectedLockerModel lockerModel)
        {
            ResponseValidityModel model = null;

            model = await _lockerService.ClearLockerForBooking(lockerModel.CabinetLocationId, lockerModel.LockerTypeId,
                lockerModel.PositionId,   lockerModel.UserKeyId, lockerModel.LockerDetailId);
            if (model.MessageReturnNumber > 0)
                return Ok(model);
            else
            {
                return Conflict(model);
            }
        }






    }
}
