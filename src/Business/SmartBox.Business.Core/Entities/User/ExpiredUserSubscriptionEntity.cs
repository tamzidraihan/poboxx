using SmartBox.Business.Core.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.User
{
    public class UserSubscriptionExpirationEntity
    {
        public int Id { get; set; }
        public string UserKeyId { get; set; }
        public int UserId { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string LockerTypeDescription { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public DeviceType UserDeviceType { get; set; }
        public string UserToken { get; set; }
        public DateTime ExpiryDate { get; set; }
       
    }
}
