using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Company
{
    public class CompanyModel: CompanyBaseModel
    {
        public List<int> CabinetLocationIds { get; set; }
    }
}
