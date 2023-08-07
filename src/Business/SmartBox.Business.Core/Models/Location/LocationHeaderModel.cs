using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Location
{
    public class LocationHeaderModel
    {
        public LocationHeaderModel()
        {
            ValidityModel = new ResponseValidityModel();
            RegionList = new List<PHRegionModel>();
            ProvinceList = new List<PHProvinceModel>();
            CitiesList = new List<PHCityModel>();
            BarangayList = new List<PHBarangayModel>();
        }

        public ResponseValidityModel ValidityModel { get; set; }
        public List<PHRegionModel> RegionList { get; set; }
        public List<PHProvinceModel> ProvinceList { get; set; }
        public List<PHCityModel> CitiesList { get; set; }
        public List<PHBarangayModel> BarangayList { get; set; }

    }
}
