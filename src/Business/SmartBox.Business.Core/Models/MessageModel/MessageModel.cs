using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.MessageModel
{
    public class MessageModel
    {
        public int ApplicationMessageId { get; set; }
        public int EmailMessageId { get; set; }
        public string Message { get; set; }
        public string HtmlBody { get; set; }
        public string Subject { get; set; }
    }
}
