﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Maintenance
{
    public class MaintenanceInspectionTestingModel
    {
        public int Id { get; set; }
        public int LockerDetailId { get; set; }
        public int CompanyId { get; set; }
        public int CabinetId { get; set; }
        public int LockerTypeId { get; set; }
        public string LockerNumber { get; set; }
        public int MaintenanceReasonTypeId { get; set; }
        public int TypeId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
