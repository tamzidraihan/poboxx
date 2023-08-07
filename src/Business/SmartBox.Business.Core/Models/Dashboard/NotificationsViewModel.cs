using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Dashboard
{
    public class NotificationsViewModel
    {
        public List<TodayNotifications> TodayNotifications { get; set; }
        public List<YesterdayNotifications> YesterdayNotifications { get; set; }
    }
}
