using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.User
{
    public class UserSubscriptionModel
    {
        public int LockerDetailId { get; set; }
        public string UserKeyId { get; set; }
        public int CabinetLocationId { get; set; }
        public int LockerTypeId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public DateTime NextBillingDate { get; set; }
        public int LockerTransactionsId { get; set; }
    }
}
