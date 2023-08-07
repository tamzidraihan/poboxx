using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public enum MessageType { PlainTextContent = 0, HtmlContent = 1 }
    public class EmailModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public MessageType Type { get; set; }
    }
}
