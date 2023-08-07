using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Notification
{
    public class NotificationEntity
    {

        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int CabinetLocationId { get; set; }
        public string Description { get; set; } 
        public DateTime DateCreated { get; set; }
    }
}
