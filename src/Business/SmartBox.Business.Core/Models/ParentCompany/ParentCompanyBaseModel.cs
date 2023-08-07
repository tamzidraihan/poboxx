using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.ParentCompany
{
    public class ParentCompanyBaseModel
    {
        public string ParentCompanyKeyId { get; set; }
        public string ParentCompanyName { get; set; }
        public byte[] ParentCompanyLogo { get; set; }
        public string ParentCompanyAddress { get; set; }
        public string ParentCompanyContactPerson { get; set; }
        public string ParentCompanyContactNumber { get; set; }
    }
}
