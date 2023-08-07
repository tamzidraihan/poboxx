using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Cabinet
{
    public class CabinetLocationViewModel:CabinetLocationModel
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string RegionName { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string BarangayName { get; set; }
    }
}
