using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class ExtendLockerBookingModel
    {
        public int LockerTransactionsId { get; set; }
        public decimal NewTotalPrice { get; set; }
        public DateTime NewStorageEndDate { get; set; }
        public DateTime StartStorageDateTime { get; set; }

        public DateTime EndStorageDateTime { get; set; }
    }
    public class CancelBookingModel
    {
        public int lockerTransactionsId { get; set; }
        public string paymentReferenceId { get; set; }
    }
}
