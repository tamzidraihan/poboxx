﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Booking
{
    public class BookingLockerDetailModel
    {
        public int lockerTransactionId { get; set; }
        public int CabinetId { get; set; }
        public string lockerNumber { get; set; }
    }
}
