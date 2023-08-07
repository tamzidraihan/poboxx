using SmartBox.Business.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Report
{
    public enum CleanlinessReportStatus { Pending = 0, Overdue = 1, Done = 2 }
    public enum Month { Jan = 1, Feb = 2, Mar = 3, Apr = 4, May = 5, June = 6, July = 7, Aug = 8, Sep = 9, Oct = 10, Nov = 11, Dec = 12 }
    public class CleanlinessReportEntity : CommonFields
    {  
        public int CompanyId { get; set; }
        public int CabinetId { get; set; }
        public string FrontPhoto { get; set; }
        public string RightPhoto { get; set; }
        public string LeftPhoto { get; set; }
        public string Message { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public Month Month { get; set; }
        public CleanlinessReportStatus? Status { get; set; }
        public List<LockerPictureEntity> LockerPictures { get; set; }
        public bool IsSubmitted { get; set; }
        public string CompanyName { get; set; }
        public string CompanyBusinessName { get; set; }
        public string CompanyOwnerName { get; set; }
        public string LockerNumber { get; set; }
        public string InsidePhoto { get; set; }
        public string OutsidePhoto { get; set; }
    }
    
}
