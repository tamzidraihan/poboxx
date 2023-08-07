using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Booking
{
    public class ShortLockerTransactionModel
    {
        public int lockerDetailId { get; set; }
        public int lockerTransactionId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string LockerNumber { get; set; }
        public int accessPlan { get; set; }
    }
}
