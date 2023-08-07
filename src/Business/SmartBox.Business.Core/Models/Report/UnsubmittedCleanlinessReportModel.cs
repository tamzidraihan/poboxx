using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Report
{
    public class UnsubmittedMaintenanceReportModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
    }
}
