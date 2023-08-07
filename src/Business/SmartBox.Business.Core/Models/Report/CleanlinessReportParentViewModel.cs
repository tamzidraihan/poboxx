using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Report
{
    public class CleanlinessReportParentViewModel
    {
        public int Count { get; set; }

        public List<CleanlinessReportViewModel> CleanlinessReportViewModel { get; set; }
    }
}
