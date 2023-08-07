using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Pricing
{
    public class PriceAndChargingModel
    {
        public int Id { get; set; }
        public int PricingMatrixId { get; set; }
        public int LocationId { get; set; }
        public int LockerTypeId { get; set; }
        public decimal StoragePrice { get; set; }
        public decimal MultiAccessStoragePrice { get; set; }
        public decimal OverstayCharge { get; set; }
        public string LockerTypeSize { get; set; }
        public string LockerTypeDescription { get; set; }
        public string cabinetlocationDescription { get; set; }
    }
}
