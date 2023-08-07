using SmartBox.Business.Core.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public class UserTokenModel
    {
        public DeviceType DeviceType { get; set; }
        public string Token { get; set; }
    }
}
