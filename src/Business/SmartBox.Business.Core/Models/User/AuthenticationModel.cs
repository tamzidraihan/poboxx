using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.User
{
    public class AuthenticationModel 
    {
        public AuthenticationModel()
        {
           ValidityModel = new ResponseValidityModel();    
        }
        public string Token { get; set; }
        public string OneSignalId { get; set; }
        public string OneSignalHash { get; set; }
        public ResponseValidityModel ValidityModel { get; set; }
    }
}
