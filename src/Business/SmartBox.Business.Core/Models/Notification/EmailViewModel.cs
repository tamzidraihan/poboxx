using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Notification
{
    public class EmailViewModel:EmailModel
    {
        public bool IsDeleted { get; set; }
    }
}
