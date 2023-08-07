using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Shared
{
    public class GlobalConfigurations
    {
        public string ContactNo { get; set; }
        public int DaysAllowedAfterExpired { get; set; }
        public string ReceiptEmailSubject { get; set; }
        public string AppName { get; set; }
        public string BaseApiUrl { get; set; }
    }
}
