using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Booking
{
    public class BookingTransactionsViewModel
    {
        public int TotalRecordCount { get; set; }
        public int LockerTransactionsId { get; set; }
        public int LockerTransactionTypeId { get; set; }
        public string UserKeyId { get; set; }
        public int CompanyId { get; set; }
        public int LockerDetailId { get; set; }  
        public int LockerTypeId { get; set; }
        public int PositionId { get; set; }
        public int CabinetLocationId { get; set; }
        public string LockerNumber { get; set; } 
        public DateTime StoragePeriodStart { get; set; }
        public DateTime StoragePeriodEnd { get; set; }
        public string SenderName { get; set; }
        public string SenderMobile { get; set; }
        public string SenderEmailAddress { get; set; }
        public DateTime DropOffDate { get; set; }
        public string DateTime { get; set; }
        public string DropOffQRCode { get; set; }
        public DateTime PickupDate { get; set; }
        public string PickUpCode { get; set; }
        public string PickUpQRCode { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public int TotalPrice { get; set; }
        public string PaymentReference { get; set; }
        public int BookingStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string LocationDescription { get; set; }
        public string LocationAdress { get; set; }
        public string CompanyFirstName { get; set; }
        public string CompanyMiddleName { get; set; }
        public string CompanyBusinessName { get; set; }
        public string CompanyEmail { get; set; }
        public decimal TotalStorageTime { get; set; }
        public decimal StorageFee { get; set; }
        public decimal Credit { get; set; }
        public decimal Discount { get; set; }
        public decimal Charges { get; set; } 
        public decimal FranchiseeFee { get; set; }
        public decimal NetCollection { get; set; }
        public string ModeOfPayment { get; set; }
        public DateTime? DateOfPayment { get; set; }
        public string LockerTypeDescription { get; set; }
        public string LockerSize { get; set; }
        public int AccessPlan { get; set; }


    }
}
