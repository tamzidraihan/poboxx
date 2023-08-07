using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class AvailableLockerTypeModel
    {
        //public int CabinetId { get; set; }
        public int LockerTypeId { get; set; }   
        public string LockerTypeDescription { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int NumberOfAvailable { get; set; }
        public int PositionId { get; set; }
    }
}
