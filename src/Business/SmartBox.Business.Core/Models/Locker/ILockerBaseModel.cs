using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public interface ILockerBaseModel<T>
    {
        public int CabinetId { get; set; }
        public int LockerTypeId { get; set; }
        public int LockerDetailId { get; set; }
        public string LockerNumber { get; set; }
        public string BoardNumber { get; set; }
        
    }
}
