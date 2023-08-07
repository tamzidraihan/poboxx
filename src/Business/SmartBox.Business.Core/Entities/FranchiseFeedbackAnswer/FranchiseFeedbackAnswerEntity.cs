using SmartBox.Business.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.FranchiseFeedbackAnswer
{
    public class FranchiseFeedbackAnswerEntity : BaseEntity
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public int CompanyId { get; set; }
        public int QuestionId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
