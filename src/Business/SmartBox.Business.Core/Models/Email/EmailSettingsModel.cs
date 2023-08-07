using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Email
{
    public class EmailSettingsModel
    {
        public string Key { get; set; }
        public string From { get; set; }
        public string SenderName { get; set; }
    }
}
