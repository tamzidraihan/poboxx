using SmartBox.Business.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Notification
{
    public enum DeviceType { Android = 0, IOS = 1, Web = 2, Other = 3 }
    public class UserTokenEntity : CommonFields
    {
        public int UserId { get; set; }
        public DeviceType DeviceType { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string URL { get; set; }
        public bool IsEnable { get; set; }
       
    }
}
