using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Booking
{
    public class NewBookingCheckOutModel
    {
        public double TotalAmount { get; set; }
        public string ItemName { get; set; }
        public string SelectedCurrency { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<BookingDetailForCheckOutModel> Bookings { get; set; }

    }
    public class BookingDetailForCheckOutModel
    {
        public int LockerDetailId { get; set; }
        public DateTime StoragePeriodStart { get; set; }
        public DateTime StoragePeriodEnd { get; set; }

    }
    public class BookingExtentionCheckOutModel
    {
        public double TotalAmount { get; set; }
        public string ItemName { get; set; }
        public string SelectedCurrency { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int lockerTransactionId { get; set; }
        public DateTime NewStoragePeriodEnd { get; set; }
    }
    public class BookingCancelCheckOutModel
    {
        public double TotalAmount { get; set; }
        public string ItemName { get; set; }
        public string SelectedCurrency { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int lockerTransactionId { get; set; }
    }
}
