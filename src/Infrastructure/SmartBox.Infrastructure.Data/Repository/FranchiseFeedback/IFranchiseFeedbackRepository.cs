using SmartBox.Business.Core.Entities.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Entities.FranchiseFeedbackQuestion;
using SmartBox.Business.Core.Models.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Models.FranchiseFeedbackQuestion;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.FranchiseFeedbackQuestion
{
    public interface IFranchiseFeedbackRepository : IGenericRepositoryBase<FranchiseFeedbackQuestionEntity>
    {


        Task<int> Save(FranchiseFeedbackQuestionEntity franchiseFeedbackQuestion);
        Task<FranchiseFeedbackQuestionEntity> GetById(int Id);
        Task<FranchiseFeedbackQuestionEntity> GetByQuestion(string question);
        Task<List<FranchiseFeedbackQuestionViewModel>> GetFranchiseFeedbackQuestions();
        Task<int> DeleteFranchiseFeedbackQuestion(int Id);
       

        //Answer
        Task<int> FeedbackAnswerSave(FranchiseFeedbackAnswerEntity franchiseFeedbackAnswer);
        Task<FranchiseFeedbackAnswerEntity> GetByAnswer(string answer);
        Task<List<FranchiseFeedbackAnswerViewModel>> GetFranchiseFeedbackAnswer();
        Task<int> DeleteFranchiseFeedbackAnswer(int Id);




    }
}
