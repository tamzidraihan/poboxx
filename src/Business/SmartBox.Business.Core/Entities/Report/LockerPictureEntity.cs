using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Report
{
    public class LockerPictureEntity
    {
        public int Id { get; set; }
        public string LockerNumber { get; set; }
        public string InsidePhoto { get; set; }
        public string OutsidePhoto { get; set; }
        public int CleanlinessReportId { get; set; }
    }
}
