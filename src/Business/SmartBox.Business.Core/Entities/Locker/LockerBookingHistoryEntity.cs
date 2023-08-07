using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Locker
{
    public class LockerBookingHistoryEntity
    {
        public int LockerTransactionsId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}
