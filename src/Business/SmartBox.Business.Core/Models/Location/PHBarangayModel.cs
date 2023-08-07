using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Location
{
    public class PHBarangayModel
    {
        public string brgyCode { get; set; }
        public string brgyDesc { get; set; }
        public string regCode { get; set; }
        public string provCode { get; set; }
        public string citymunCode { get; set; }
        public string error_message { get; set; }
    }
}
