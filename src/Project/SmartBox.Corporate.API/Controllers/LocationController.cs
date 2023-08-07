using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmartBox.Business.Core.Models.Location;
using SmartBox.Business.Services.Service.Location;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SmartBox.Corporate.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }


        /// <summary>
        /// Get a list of region
        /// </summary>
        /// <response code="200">LocationHeaderModel</response>
        [HttpGet("GetRegion")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(LocationHeaderModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<LocationHeaderModel>> GetRegion()
        {
            var model = await _locationService.GetRegions();
            return Ok(model);
        }

        /// <summary>
        /// Get a list of province based on regionId
        /// </summary>
        /// <response code="200">LocationHeaderModel</response>
        [HttpGet("GetProvince")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(LocationHeaderModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<LocationHeaderModel>> GetProvince([BindRequired] string regionId)
        {
            var model = await _locationService.GetProvinces(regionId);
            return Ok(model);
        }

        /// <summary>
        /// Get a list of cities based on provinceId
        /// </summary>
        /// <response code="200">LocationHeaderModel</response>
        [HttpGet("GetCities")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(LocationHeaderModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<LocationHeaderModel>> GetCities([BindRequired] string provinceId)
        {
            var model = await _locationService.GetCities(provinceId);
            return Ok(model);
        }

        /// <summary>
        /// Get a list of barangays based on cityId
        /// </summary>
        /// <response code="200">LocationHeaderModel</response>
        [HttpGet("GetBarangays")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(LocationHeaderModel), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<LocationHeaderModel>> TestLocation([BindRequired] string cityId)
        {
            var model = await _locationService.GetBarangays(cityId);
            return Ok(model);
        }
    }
}
