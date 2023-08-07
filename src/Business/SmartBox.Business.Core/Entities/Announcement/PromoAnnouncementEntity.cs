using SmartBox.Business.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Announcement
{
    public class PromoAnnouncementEntity : CommonFields
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int AnnouncementTypeId { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public string ExternalUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
