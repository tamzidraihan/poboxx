using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class LockerManualBookingModel: BaseBookingModel<LockerManualBookingModel>
    {
        public string UserKeyId { get; set; }
        public string SenderName { get; set; }
        public string SenderMobile { get; set; }
        public string SenderEmailAddress { get; set; }
        public int LockerDetailId { get; set; }

    }
}
