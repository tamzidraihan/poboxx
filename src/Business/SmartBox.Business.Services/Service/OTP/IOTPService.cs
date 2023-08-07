using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.OTP
{
    public interface IOTPService
    {
        Task<string> GenerateOTP();
        DateTime OTPExpiration();
    }
}
