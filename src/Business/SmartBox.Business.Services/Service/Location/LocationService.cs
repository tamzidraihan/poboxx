using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SmartBox.Business.Core.Models.TextValue;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Models.Location;
using SmartBox.Business.Core;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using SmartBox.Business.Core.Models.ResponseValidity;

namespace SmartBox.Business.Services.Service.Location
{
    public class LocationService : BaseMessageService<ILocationService>, ILocationService
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment hostEnvironment;

        public LocationService(IAppMessageService appMessageService, IMapper mapper,
            ILogger<LocationService> logger, IHostEnvironment hostEnvironment) : base(appMessageService, mapper)
        {
            _logger = logger;
            this.hostEnvironment = hostEnvironment;
        }
        public async Task<LocationHeaderModel> GetRegions()
        {
            var fileName = "wwwroot/ph-location-files/regions.json";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await Shared.SharedServices.ReadFile(fullPath);
            var model = new LocationHeaderModel();
            if (response.isError)
            {
                model.ValidityModel.Message = response.ErrorMessage;
                return model;
            }
            try
            {
                var regions = JsonConvert.DeserializeObject<List<PHRegionModel>>(response.Content);
                model.RegionList.AddRange(regions);
            }
            catch (Exception ex)
            {
                model.ValidityModel.Message = ex.Message;
            }
            return model;
        }

        public async Task<PHRegionModel> GetRegion(string regionCode)
        {
            var fileName = "wwwroot/ph-location-files/regions.json";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await Shared.SharedServices.ReadFile(fullPath);
            var model = new PHRegionModel();
            if (response.isError)
            {
                model.error_message = response.ErrorMessage;
                return model;
            }
            try
            {
                var regions = JsonConvert.DeserializeObject<List<PHRegionModel>>(response.Content);
                var region = regions.Where(s => s.region_code == regionCode).FirstOrDefault();
                if (region != null) return region;
            }
            catch (Exception ex)
            {
                model.error_message = ex.Message;
            }
            return model;
        }

        public async Task<LocationHeaderModel> GetProvinces(string regionCode)
        {
            var fileName = "wwwroot/ph-location-files/provinces.json";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await Shared.SharedServices.ReadFile(fullPath);
            var model = new LocationHeaderModel();
            if (response.isError)
            {
                model.ValidityModel.Message = response.ErrorMessage;
                return model;
            }
            try
            {
                var allProvinces = JsonConvert.DeserializeObject<List<PHProvinceModel>>(response.Content);
                if (allProvinces.Count > 0)
                {
                    var filterProvinces = allProvinces.Where(s => s.region_code == regionCode).ToList();
                    model.ProvinceList.AddRange(filterProvinces);
                    model.ValidityModel = this.AppMessageService.SetFoundMessage().MappedResponseValidityModel();
                }
                else
                    model.ValidityModel = this.AppMessageService.SetNotFoundMessage().MappedResponseValidityModel();
            }
            catch (Exception ex)
            {
                model.ValidityModel.Message = ex.Message;
            }
            return model;
        }

        public async Task<PHProvinceModel> GetProvince(string provinceCode)
        {
            var fileName = "wwwroot/ph-location-files/provinces.json";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await Shared.SharedServices.ReadFile(fullPath);
            var model = new PHProvinceModel();
            if (response.isError)
            {
                model.error_message = response.ErrorMessage;
                return model;
            }
            try
            {
                var allProvinces = JsonConvert.DeserializeObject<List<PHProvinceModel>>(response.Content);
                var filterProvince = allProvinces.Where(s => s.province_code == provinceCode).FirstOrDefault();
                if (filterProvince != null) return filterProvince;
            }
            catch (Exception ex)
            {
                model.error_message = ex.Message;
            }
            return model;
        }
        public async Task<LocationHeaderModel> GetCities(string provinceCode)
        {
            var fileName = "wwwroot/ph-location-files/cities.json";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await Shared.SharedServices.ReadFile(fullPath);
            var model = new LocationHeaderModel();
            if (response.isError)
            {
                model.ValidityModel.Message = response.ErrorMessage;
                return model;
            }
            try
            {
                var allCities = JsonConvert.DeserializeObject<List<PHCityModel>>(response.Content);
                if (allCities.Count > 0)
                {
                    var filterCities = allCities.Where(s => s.province_code == provinceCode).ToList();
                    model.CitiesList.AddRange(filterCities);
                    model.ValidityModel = this.AppMessageService.SetFoundMessage().MappedResponseValidityModel();
                }
                else
                    model.ValidityModel = this.AppMessageService.SetNotFoundMessage().MappedResponseValidityModel();
            }
            catch (Exception ex)
            {
                model.ValidityModel.Message = ex.Message;
            }
            return model;
        }

        public async Task<PHCityModel> GetCity(string cityCode)
        {
            var fileName = "wwwroot/ph-location-files/cities.json";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await Shared.SharedServices.ReadFile(fullPath);
            var model = new PHCityModel();
            if (response.isError)
            {
                model.error_message = response.ErrorMessage;
                return model;
            }
            try
            {
                var allCities = JsonConvert.DeserializeObject<List<PHCityModel>>(response.Content);
                var filterCity = allCities.Where(s => s.city_code == cityCode).FirstOrDefault();
                if (filterCity != null) return filterCity;
            }
            catch (Exception ex)
            {
                model.error_message = ex.Message;
            }
            return model;
        }
        public async Task<LocationHeaderModel> GetBarangays(string cityCode)
        {
            var fileName = "wwwroot/ph-location-files/brgys.json";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await Shared.SharedServices.ReadFile(fullPath);
            var model = new LocationHeaderModel();
            if (response.isError)
            {
                model.ValidityModel.Message = response.ErrorMessage;
                return model;
            }
            try
            {
                var allBarangays = JsonConvert.DeserializeObject<List<PHBarangayModel>>(response.Content);
                if (allBarangays.Count > 0)
                {
                    var filterBarangays = allBarangays.Where(s => s.citymunCode == cityCode).ToList();
                    model.BarangayList.AddRange(filterBarangays);
                    model.ValidityModel = this.AppMessageService.SetFoundMessage().MappedResponseValidityModel();
                }
                else
                    model.ValidityModel = this.AppMessageService.SetNotFoundMessage().MappedResponseValidityModel();
            }
            catch (Exception ex)
            {
                model.ValidityModel.Message = ex.Message;
            }
            return model;
        }
        public async Task<PHBarangayModel> GetBarangay(string barangayCode)
        {
            var fileName = "wwwroot/ph-location-files/brgys.json";
            var fullPath = Path.Combine(hostEnvironment.ContentRootPath, fileName);
            var response = await Shared.SharedServices.ReadFile(fullPath);
            var model = new PHBarangayModel();
            if (response.isError)
            {
                model.error_message = response.ErrorMessage;
                return model;
            }
            try
            {
                var allBarangays = JsonConvert.DeserializeObject<List<PHBarangayModel>>(response.Content);
                var filterBarangay = allBarangays.Where(s => s.brgyCode == barangayCode).FirstOrDefault();
                if (filterBarangay != null) return filterBarangay;
            }
            catch (Exception ex)
            {
                model.error_message = ex.Message;
            }
            return model;
        }

       
    }
}
