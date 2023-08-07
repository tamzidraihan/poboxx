using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Company
{
    public  class CompanyViewModel : CompanyBaseModel
    {
        public int CompanyId { get; set; }
        public List<CompanyLocationModel> CompanyBranches { get; set; }
    }
}
