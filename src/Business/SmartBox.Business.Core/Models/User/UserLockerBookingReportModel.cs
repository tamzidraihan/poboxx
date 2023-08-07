using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.User
{
    public class UserLockerBookingReportModel
    {
        public List<MonthlyDetail> MonthlyDetail { get; set; }
        public CurrentMonthDetail CurrentMonthDetail { get; set; }
        public int TotalBookings { get; set; }
    }
    public class MonthlyDetail
    {
        public int TotalBookings { get; set; }
        public int Month { get; set; }
    }
    public class CurrentMonthDetail
    {
        public int DropOffCount { get; set; }
        public int PickUpCount { get; set; }
        public int ConfiscatedCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
