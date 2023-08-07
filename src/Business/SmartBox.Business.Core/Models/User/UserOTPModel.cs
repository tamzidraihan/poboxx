using SmartBox.Business.Core.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.User
{
   public class UserOTPModel 
    {
        public string UserKeyId { get; set; }
        public string PhoneNumber { get; set; }
        public string NewEmail { get; set; }
        public bool IsEmailOTPOnly { get; set; }
 
    }
}
