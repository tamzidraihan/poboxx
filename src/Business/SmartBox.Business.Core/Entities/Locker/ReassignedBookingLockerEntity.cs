using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Locker
{
    public class ReassignedBookingLockerEntity
    {
        public int Id { get; set; }
        public int? ReassignedByAdminUser { get; set; }
        public string AdminFirstName { get; set; }
        public string AdminLastName { get; set; }
        public int? ReassignedByCompanyUser { get; set; }
        public string CompanyUserFirstName { get; set; }
        public string CompanyUserLastName { get; set; }
        public int? CompanyId { get; set; }
        public int lockerTransactionsId { get; set; }
        public int OldLockerDetailId { get; set; }
        public int NewLockerDetailId { get; set; }
        public DateTime ReassignmentDate { get; set; }
        public int OldCabinetId { get; set; }
        public string OldLockerNumber { get; set; }
        public int NewCabinetId { get; set; }
        public string NewLockerNumber { get; set; }
    }
}
