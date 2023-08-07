using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Company
{
    public class CompanyLocationEntity : CompanyEntity
    {
        public int CabinetLocationId { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CabinetLocationAddress { get; set; }
        public bool IsCabinetLocationDeleted { get; set; }
    }
}
