using SmartBox.Business.Core.Entities.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Report
{
    public class CleanlinessReportViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int CabinetId { get; set; }
        public string FrontPhoto { get; set; }
        public string RightPhoto { get; set; }
        public string LeftPhoto { get; set; }
        public string Message { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public Month Month { get; set; }
        public CleanlinessReportStatus? Status { get; set; }
        public List<LockerPictureModel> LockerPictures { get; set; }
        public bool IsSubmitted { get; set; }
        public string CompanyName { get; set; }
        public string CompanyBusinessName { get; set; }
        public string CompanyOwnerName { get; set; } 
       
    }
}
