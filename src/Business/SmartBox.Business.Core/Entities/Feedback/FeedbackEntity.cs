using SmartBox.Business.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Feedback
{
    public class FeedbackEntity: BaseEntity
    {
        public int FeedbackId { get; set; } 
        public string UserKeyId { get; set; }
        public int AppRating { get; set; }
        public string FeaturesExpectations { get; set; }
        public int LockerEquipmentRating { get; set; }
        public string Suggestion { get; set; }
        public int LocationRating { get; set; }
        public string WantToSee { get; set; }
        public string BetterExperience { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

    }
}
