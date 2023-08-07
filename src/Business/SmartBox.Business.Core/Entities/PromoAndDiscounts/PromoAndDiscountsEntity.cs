using SmartBox.Business.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.PromoAndDiscounts
{
    public class PromoAndDiscountsEntity: BaseEntity
    {
        public int PromoAndDiscountsId { get; set; } 
        public string Description { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public string ExteralLink { get; set; }
        public decimal BookingDiscount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

    }
}
