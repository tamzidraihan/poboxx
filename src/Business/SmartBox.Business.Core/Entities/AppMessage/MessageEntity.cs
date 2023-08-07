using SmartBox.Business.Core.Entities.Base;

namespace SmartBox.Business.Core.Entities.AppMessage
{
    public class MessageEntity : BaseEntity
    {
        public int ApplicationMessageId { get; set; }
        public int EmailMessageId { get; set; }
        public string Message { get; set; }
        public string HtmlBody { get; set; }
        public string Subject { get; set; }   
    }
}
