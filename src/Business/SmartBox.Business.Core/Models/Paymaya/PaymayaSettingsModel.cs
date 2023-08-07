using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Paymaya
{
    public class PaymayaSettingsModel
    {

        public int HttpsPort { get; set; }
        public string AuthorizationKey { get; set; }
        public string SuccessAuthorizeUrl { get; set; }
        public string FailedAuthorizeUrl { get; set; }
        public string CancelPaymentUrl { get; set; }
        public string SuccessPaymentUrl { get; set; }
        public string FailedPaymentUrl { get; set; }
        public string PaymentUrl { get; set; }
        public string CheckOutUrl { get; set; }


    }
}
