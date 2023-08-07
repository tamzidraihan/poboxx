using SmartBox.Business.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Announcement
{
    public class AnnouncementTypeEntity : CommonFields
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
