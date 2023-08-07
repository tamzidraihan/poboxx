using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Models.Base;

namespace SmartBox.Business.Core.Models.ResponseValidity
{
    public class ResponseValidityModel : BaseMessageModel
    {
        public ResponseValidityModel()
        {
            MessagesList = new List<string>();
        }

        public List<string> MessagesList { get; set; }

    }
}
