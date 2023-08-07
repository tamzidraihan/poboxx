using SmartBox.Business.Core.Models.Location;
using SmartBox.Business.Core.Models.TextValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Location
{
    public interface ILocationService
    {
        Task<PHRegionModel> GetRegion(string regionCode);
        Task<PHProvinceModel> GetProvince(string provinceCode);
        Task<PHCityModel> GetCity(string cityCode);
        Task<PHBarangayModel> GetBarangay(string barangayCode);

        Task<LocationHeaderModel> GetRegions();
        Task<LocationHeaderModel> GetProvinces(string regionCode);
        Task<LocationHeaderModel> GetCities(string provinceCode);
        Task<LocationHeaderModel> GetBarangays(string cityCode);
    }
}
