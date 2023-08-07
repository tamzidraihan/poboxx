using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.CompanyUser
{
    public class ResponseValidityCompanyModel : ResponseValidityModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserkeyId { get; set; }
    }
}
