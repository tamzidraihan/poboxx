using SmartBox.Business.Core.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.PayMongo
{
    public class PostPaymentSourceModel : PaymentReferenceModel
    {
        public string RedirectUrl { get; set; }
    }
}
