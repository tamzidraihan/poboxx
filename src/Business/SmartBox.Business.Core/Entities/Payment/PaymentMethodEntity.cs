using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Payment
{
    public class PaymentMethodEntity
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
