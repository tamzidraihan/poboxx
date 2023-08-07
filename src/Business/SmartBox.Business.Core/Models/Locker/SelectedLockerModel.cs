using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public  class SelectedLockerModel
    {
        public int CabinetLocationId { get; set; }
        public int LockerTypeId { get; set; }
        public int PositionId { get; set; }
        public DateTime storageStartDate { get; set; }
        public DateTime StorageEndDate { get; set; }
        public int LockerDetailId { get; set; }
        public string  UserKeyId { get; set; }
    }
}
