using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;

namespace SmartBox.Business.Core.Models.User
{
    public class UserModel: UserFormModel
    {
        public string UserKeyId { get; set; }
        public string OTP { get; set; }
        public DateTime OTPExpirationDate { get; set; }
        public string MPIN { get; set; }
        
        public List<int> RoleId { get; set; }
    }
}
