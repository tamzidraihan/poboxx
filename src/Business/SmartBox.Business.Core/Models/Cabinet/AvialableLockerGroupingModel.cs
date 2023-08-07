using SmartBox.Business.Core.Models.Locker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Cabinet
{
    public class AvialableLockerGroupingModel
    {
        public int PositionId { get; set; }
        public List<UpdatedAvailableLockerModel> Details { get; set; }
    }
}
