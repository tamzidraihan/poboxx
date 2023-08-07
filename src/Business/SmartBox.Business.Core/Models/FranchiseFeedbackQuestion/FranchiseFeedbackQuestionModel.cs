using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Core.Models.FranchiseFeedbackQuestion
{
    public class FranchiseFeedbackQuestionModel
    {
        public int Id { get; set; }
        public string Question { get; set; } 
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public FranchiseFeedbackQuestionType? Type { get; set; }
    }
}
