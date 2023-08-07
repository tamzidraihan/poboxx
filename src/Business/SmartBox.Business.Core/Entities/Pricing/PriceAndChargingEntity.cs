using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Pricing
{
    public class PriceAndChargingEntity
    {
        public int Id { get; set; }
        public int PricingMatrixId { get; set; }
        public int LocationId { get; set; }
        public int LockerTypeId { get; set; }
        public decimal StoragePrice { get; set; }
        public decimal MultiAccessStoragePrice { get; set; }
        public decimal OverstayCharge { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string PricingType { get; set; }
        public int OverstayPeriod { get; set; }
        public string LockerTypeSize { get; set; }
        public string LockerTypeDescription { get; set; }
        public string cabinetlocationDescription { get; set; }

    }
}
