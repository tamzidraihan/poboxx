using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.User
{
    public class UserSubscriptionViewModel
    {
        public int Id { get; set; }
        public int LockerDetailId { get; set; }
        public string UserKeyId { get; set; }
        public int CabinetLocationId { get; set; }
        public int LockerTypeId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string LockerTypeDescription { get; set; }
        public DateTime NextBillingDate { get; set; }
        public string LockerNumber { get; set; }
        public int LockerTransactionsId { get; set; }

    }
}
