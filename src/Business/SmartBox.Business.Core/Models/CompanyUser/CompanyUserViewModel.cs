using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.CompanyUser
{
    public class CompanyUserViewModel : BaseCompanyUserModel
    {
        public string UserKeyId { get; set; }
        public int CompanyUserId { get; set; }
    }
}
