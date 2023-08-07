using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Email
{
    public class SendEmailModel
    {
        public string To { get; set; }
        public string ReceiverName { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainBody { get; set; }
    }
}
