using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.AppMessage
{
    public class ApplicationMessageModel
    {
        public int ApplicationMessageId { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
    }
}
