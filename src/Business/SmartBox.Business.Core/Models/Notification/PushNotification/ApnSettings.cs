using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification.PushNotification
{
    public class ApnSettings
    {
        /// <summary>
        /// p8 certificate string
        /// </summary>
        public string P8PrivateKey { get; set; }

        /// <summary>
        /// 10 digit p8 certificate id. Usually a part of a downloadable certificate filename
        /// </summary>
        public string P8PrivateKeyId { get; set; }

        /// <summary>
        /// Apple 10 digit team id
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// App slug / bundle name
        /// </summary>
        public string AppBundleIdentifier { get; set; }

        /// <summary>
        /// Development or Production server
        /// </summary>
        public ApnServerType ServerType { get; set; }
    }
    public enum ApnServerType
    {
        Development,
        Production
    }
}
