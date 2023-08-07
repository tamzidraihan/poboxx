using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class BaseLockerModel<T> : ILockerBaseModel<T> where T : class
    {
        public int LockerDetailId { get; set; }
        public int CabinetId { get; set; }
        public int LockerTypeId { get; set; }
        public string LockerNumber { get; set; }
        public string BoardNumber { get; set; }
        public int PositionId { get; set; }
    }
}
