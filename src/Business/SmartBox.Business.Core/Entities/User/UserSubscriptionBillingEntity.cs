using SmartBox.Business.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.User
{
    public class UserSubscriptionBillingEntity : CommonFields
    {
        public int UserSubscriptionId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
