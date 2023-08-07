using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Dashboard
{
    public class DashboardViewModel
    {
        public string CompanyId { get; set; }
        public string Bookings { get; set; }
        public string Locations { get; set; }
        public string Drops { get; set; }
        public string Deliveries { get; set; }

        public List<RecentBookingsViewModel> RecentBookings { get; set; }
        public List<MostBookedLockerViewModel> MostBookedLocker { get; set; }
        public List<RevenueViewModel> Revenue { get; set; }

        public NotificationsViewModel Notifications { get; set; }
    }
}
