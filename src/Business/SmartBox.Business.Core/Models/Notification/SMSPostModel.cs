using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public class SMSPostModel
    {
        public string app_key { get; set; }
        public string app_secret { get; set; }
        public string msisdn { get; set; }
        public string content { get; set; }
        public string shortcode_mask { get; set; }
        public string rcvd_transid { get; set; }
        public bool is_intl { get; set; } = false;
    }
}
