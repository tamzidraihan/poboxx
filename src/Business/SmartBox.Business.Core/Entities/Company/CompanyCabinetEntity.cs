using SmartBox.Business.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Company
{
    public class CompanyCabinetEntity
    {
        public int CompanyCabinetId { get; set; }
        public int CompanyId { get; set; }
        public int CabinetId { get; set; }
        public int IsActive { get; set; }
        public int CabinetLocationId { get; set; }
        public string CabinetNumber { get; set; }
        public int NumberOfLocker { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsAssigned { get; set; }

    }
}
