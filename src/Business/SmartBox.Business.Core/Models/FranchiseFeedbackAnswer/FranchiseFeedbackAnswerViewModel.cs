using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Business.Core.Models.FranchiseFeedbackAnswer
{
    public class FranchiseFeedbackAnswerViewModel: FranchiseFeedbackAnswerModel
    {
        public string Question { get; set; }
        public string CompanyName { get; set; }
        public string BusinessName { get; set; }
        public bool IsDeleted { get; set; }
        public FranchiseFeedbackQuestionType? Type { get; set; }
    }
}
