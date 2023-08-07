using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public class OTPModel : BaseModel
    {
        public string OTP { get; set; }
        public string QRCode { get; set; }
        public int LockerTransactionsId { get; set; }
        public string LockerNumber { get; set; }
        public string BoardNumber   { get; set; }
        public string GetStatusCommand { get; set; }
        public string OpenCommand { get; set; }
    }
}
