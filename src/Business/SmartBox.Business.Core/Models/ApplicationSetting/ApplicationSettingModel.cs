using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.ApplicationSetting
{
    public class ApplicationSettingModel
    {
        public short ApplicationSettingsId { get; set; }
        public int OTPMinutes { get; set; }
        public short OTPLength { get; set; }
        public bool IsOTPMixCharacters { get; set; }
        public string DefaultCompanyPassword { get; set; }
        public int AvailableLockerTypeNumberOfDays { get; set; }
        public int MaintainanceReportReminderDay { get; set; }  
        public string MaintainanceReportReminderHour { get; set; }
        public int MaintainanceOverdueReportReminderDay { get; set; }
        public string MaintainanceOverdueReportReminderHour { get; set; }
        public decimal? MaxRadius { get; set; }

    }
}
