using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Cabinet
{
    public class CabinetTypeEntity
    {
        public int CabinetTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconImage{ get; set; }  
        public short? IsDeactivated { get; set; }
    }
}
