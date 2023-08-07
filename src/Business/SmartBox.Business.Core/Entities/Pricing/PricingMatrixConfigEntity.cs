using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Pricing
{
    public class PricingMatrixConfigEntity
    {
        public int Id { get; set; }
        public int PricingTypeId { get; set; }
        public int OverstayPeriod { get; set; }
        public bool IsPromoEnabled { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string Name { get; set; }
    }
}
