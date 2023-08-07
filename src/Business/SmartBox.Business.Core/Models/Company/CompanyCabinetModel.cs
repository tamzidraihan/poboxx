using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Company
{
    public class CompanyCabinetModel
    {
        public int CompanyCabinetId { get; set; }
        public int CompanyId { get; set; }
        public int CabinetId { get; set; }
        public bool IsActive { get; set; }
    }
}
