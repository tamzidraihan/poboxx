using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Cabinet
{
    public class LockerTypeEntity
    {
        public int LockerTypeId { get; set; }  
        public string Description { get; set;}
        public string Size { get; set; }
        public double Price{ get; set; }
        public bool IsDeleted { get; set; }
    }
}
