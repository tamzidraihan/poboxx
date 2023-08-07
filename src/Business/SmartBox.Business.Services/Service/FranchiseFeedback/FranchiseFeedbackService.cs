using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Entities.FranchiseFeedbackQuestion;
using SmartBox.Business.Core.Models.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Models.FranchiseFeedbackQuestion;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.FranchiseFeedbackQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.FranchiseFeedbackQuestion
{
   
    public class FranchiseFeedbackService : BaseMessageService<FranchiseFeedbackService>, IFranchiseFeedbackService
    {
        private readonly IFranchiseFeedbackRepository _franchiseFeedbackRepository;
        public FranchiseFeedbackService(IAppMessageService appMessageService, IMapper mapper, IFranchiseFeedbackRepository franchiseFeedbackRepository) : base(appMessageService, mapper)
        {
            _franchiseFeedbackRepository = franchiseFeedbackRepository;
        }


         ResponseValidityModel ValidateFeedback(FranchiseFeedbackQuestionModel franchiseFeedbackQuestionModel)
        {
            var model = new ResponseValidityModel();


            if (!franchiseFeedbackQuestionModel.Question.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.FeedbackQuestionModel.Question);
            }


            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

            return model;
        }

        public async Task<ResponseValidityModel> Save(FranchiseFeedbackQuestionModel model)
        {
            var dataModel = ValidateFeedback(model);

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<FranchiseFeedbackQuestionModel, FranchiseFeedbackQuestionEntity>(model);
                var ret = await _franchiseFeedbackRepository.Save(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }


        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _franchiseFeedbackRepository.DeleteFranchiseFeedbackQuestion(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<List<FranchiseFeedbackQuestionViewModel>> GetAllQuestionsByType(GlobalEnums.FranchiseFeedbackQuestionType type)
        {
            var dbModel = await _franchiseFeedbackRepository.GetFranchiseFeedbackQuestions();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<FranchiseFeedbackQuestionViewModel>>(dbModel.Where (t=>t.Type == type).ToList());
                return model;
            }

            return new List<FranchiseFeedbackQuestionViewModel>();
        }

        public async Task<List<FranchiseFeedbackQuestionViewModel>> GetAllQuestions()
        {
            var dbModel = await _franchiseFeedbackRepository.GetFranchiseFeedbackQuestions();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<FranchiseFeedbackQuestionViewModel>>(dbModel);
                return model;
            }

            return new List<FranchiseFeedbackQuestionViewModel>();
        }

        public async Task<FranchiseFeedbackQuestionViewModel> GetById(int Id)
        {
            var dbModel = await _franchiseFeedbackRepository.GetById(Id);
            if (dbModel == null)
                return new FranchiseFeedbackQuestionViewModel();

            var model = Mapper.Map<FranchiseFeedbackQuestionViewModel>(dbModel);

            return model;
        }


        //FeedbackAnswer

            ResponseValidityModel ValidateAnswerFeedback(FranchiseFeedbackAnswerModel franchiseFeedbackAnswerModel)
        {
            var model = new ResponseValidityModel();
           
            if (!franchiseFeedbackAnswerModel.Answer.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.FeedbackAnswerModel.Answer);
            }


            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

            return model;
        }

        public async Task<ResponseValidityModel> SaveAnswer(FranchiseFeedbackAnswerModel model)
        {
            var dataModel = ValidateAnswerFeedback(model);

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<FranchiseFeedbackAnswerModel, FranchiseFeedbackAnswerEntity>(model);
                var ret = await _franchiseFeedbackRepository.FeedbackAnswerSave(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }


        public async Task<ResponseValidityModel> DeleteAnswer(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _franchiseFeedbackRepository.DeleteFranchiseFeedbackAnswer(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<List<FranchiseFeedbackAnswerViewModel>> GetAllAnswerByType( GlobalEnums.FranchiseFeedbackQuestionType type)
        {
            var dbModel = await _franchiseFeedbackRepository.GetFranchiseFeedbackAnswer();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<FranchiseFeedbackAnswerViewModel>>(dbModel.Where (e=>e.Type == type).ToList());
                return model;
            }

            return new List<FranchiseFeedbackAnswerViewModel>();
        }

        public async Task<List<FranchiseFeedbackAnswerViewModel>> GetAllAnswers()
        {
            var dbModel = await _franchiseFeedbackRepository.GetFranchiseFeedbackAnswer();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<FranchiseFeedbackAnswerViewModel>>(dbModel);
                return model;
            }

            return new List<FranchiseFeedbackAnswerViewModel>();
        }


    }
}
