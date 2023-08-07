using SmartBox.Business.Core.Entities.Base;
using System;

namespace SmartBox.Business.Core.Entities.AppMessage
{
    public class ApplicationMessageEntity : BaseEntity
    {
        public int ApplicationMessageId { get; set; } 
        public string Message { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
