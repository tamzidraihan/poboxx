using SmartBox.Business.Core.Models.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Models.FranchiseFeedbackQuestion;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.FranchiseFeedbackQuestion
{
    public interface IFranchiseFeedbackService
    {
        Task<ResponseValidityModel> Save(FranchiseFeedbackQuestionModel model);
        Task<List<FranchiseFeedbackQuestionViewModel>> GetAllQuestions();
        Task<List<FranchiseFeedbackQuestionViewModel>> GetAllQuestionsByType(GlobalEnums.FranchiseFeedbackQuestionType type);
        Task<FranchiseFeedbackQuestionViewModel> GetById(int Id);
        Task<ResponseValidityModel> Delete(int Id);

        // Answer

        Task<ResponseValidityModel> SaveAnswer(FranchiseFeedbackAnswerModel model);
        Task<List<FranchiseFeedbackAnswerViewModel>> GetAllAnswers();
        Task<List<FranchiseFeedbackAnswerViewModel>> GetAllAnswerByType(GlobalEnums.FranchiseFeedbackQuestionType type);


        Task<ResponseValidityModel> DeleteAnswer(int Id);
    }
}
