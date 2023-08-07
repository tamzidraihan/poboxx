using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class AvailableLockerDetail : BaseLockerModel<AvailableLockerDetail>
    {
        public string Description { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Size { get; set; }
        public string LockerTypeDescription { get; set; }
        public int Rating { get; set; }
        public decimal Price { get; set; }
        public string UserKeyId { get; set; }
    }
}
