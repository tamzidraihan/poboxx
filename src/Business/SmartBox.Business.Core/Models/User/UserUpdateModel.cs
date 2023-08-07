using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.User
{
    public  class UserUpdateModel: UserFormModel
    {
        public string UserKeyId { get; set; }
        public string UserMPIN { get; set; }

    }
}
