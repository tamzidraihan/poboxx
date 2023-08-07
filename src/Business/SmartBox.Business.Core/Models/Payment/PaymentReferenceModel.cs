using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Payment
{
    public class PaymentReferenceModel
    {
        public string ReferenceId { get; set; }
        public bool IsSuccessful { get; set; }  
        public string Message { get; set; }
        public string SystemMessage { get; set; }
    }
}
