using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Cabinet
{
    public class CabinetViewModel : CabinetModel
    {
        public new string CabinetLocationDescription { get; set; }
        public string Address { get; set; }
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string BarangayId { get; set; }
        public string BarangayName { get; set; }
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
