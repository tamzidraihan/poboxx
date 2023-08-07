using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Company
{
    public class CompanyCabinetViewModel
    {
        public int CompanyId { get; set; }
        public int CabinetId { get; set; }
        public int CabinetLocationId { get; set; }
        public string CabinetNumber { get; set; }
        public int NumberOfLocker { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CompanyName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
