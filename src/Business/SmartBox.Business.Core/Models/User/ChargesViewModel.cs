using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.User
{
    public class ChargesViewModel
    {
        public int Id { get; set; }
        public string UserKeyId { get; set; }
        public int LockerDetailId { get; set; }
        public int LockerTypeId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal BookingAmount { get; set; }
        public decimal Charges { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentReferenceNo { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public string LockerTypeDescription { get; set; }
        public string CabinetLocationDescription { get; set; }
    }
}
