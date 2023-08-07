using SmartBox.Business.Core.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Cabinet
{
    public class CabinetModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int CabinetId { get; set; }
        public int CabinetLocationId { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string CabinetNumber { get; set; }
        public int NumberOfLocker { get; set; }
        public bool IsDeleted { get; set; }
    }
}
