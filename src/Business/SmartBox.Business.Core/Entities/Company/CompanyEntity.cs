using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Company
{

    public class CompanyEntity : CompanyBaseEntity
    {
        public List<int> CabinetLocationIds { get; set; }
        
    }
}
