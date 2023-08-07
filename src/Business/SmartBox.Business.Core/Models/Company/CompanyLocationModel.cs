using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Company
{
    public class CompanyLocationModel
    {
        public int CabinetLocationId { get; set; }
        public int CompanyId { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CabinetLocationAddress { get; set; }
        public bool IsCabinetLocationDeleted{ get; set; }
    }
}
