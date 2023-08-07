using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Payment
{
    public class PaymentInfoModel
    {
        public int PaymongoTransactionLogId { get; set; }
        public string WebhookId { get; set; }
        public string PaymentSourceId { get; set; }
        public string Type { get; set; }
        public string SourceStatus { get; set; }
        public int Amount { get; set; }
    }
}
