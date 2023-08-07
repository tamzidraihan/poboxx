using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class LockerViewModel : LockerDetailModel
    {
        public new string Size { get; set; }
        public new string LockerTypeDescription { get; set; }
        public bool IsDeletedLockerType { get; set; }
        public bool IsAvailable { get; set; }
        public new LockerStatus LockerStatus { get; set; }
    }
}
