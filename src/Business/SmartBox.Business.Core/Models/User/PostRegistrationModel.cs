﻿using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.User
{
    public class PostRegistrationModel:BaseModel
    {
        public string OTP { get; set; }
        
    }
}
