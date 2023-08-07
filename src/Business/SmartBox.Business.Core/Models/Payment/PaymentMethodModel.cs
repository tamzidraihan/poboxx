using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Payment
{
    public class PaymentMethodModel
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
