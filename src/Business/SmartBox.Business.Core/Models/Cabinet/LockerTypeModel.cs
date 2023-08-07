using SmartBox.Business.Core.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Cabinet
{
    public class LockerTypeModel
    {
        public int LockerTypeId { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
    }
}
