using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Shared
{
    public class FirebaseConfiguration
    {
        //For Andriod and Web
        public string FcmUrl { get; set; }
        public string SenderId { get; set; }
        public string ServerKey { get; set; }
        //For IOS
        public string BaseURL { get; set; }
        public string P8PrivateKey { get; set; }
        public string P8PrivateKeyId { get; set; }
        public string ServerType { get; set; }
        public string TeamId { get; set; }
        public string AppBundleIdentifier { get; set; }
        public string ApnIdHeader { get; set; }
        public int TokenExpiresMinutes { get; set; }
    }
}
