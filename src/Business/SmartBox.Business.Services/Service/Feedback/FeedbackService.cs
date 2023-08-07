using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Repository.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.Feedback
{
    public class FeedbackService:BaseMessageService<FeedbackService>, IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackService(IAppMessageService appMessageService, IMapper mapper, IFeedbackRepository feedbackRepository) : base(appMessageService, mapper)
        {
            _feedbackRepository= feedbackRepository;
        }


            ResponseValidityModel ValidateFeedback(FeedbackModel feedbackModel)
        {
            var model = new ResponseValidityModel();
             

            return model;
        }

        public async Task<ResponseValidityModel> Save(FeedbackModel model)
        {
            var dataModel =  ValidateFeedback(model); 

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<FeedbackModel, FeedbackEntity>(model);
                var ret = await _feedbackRepository.Save(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }


        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _feedbackRepository.DeleteFeedback(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public List<FeedbackViewModel> GetAll()
        {
            var dbModel =  _feedbackRepository.GetFeedbacks();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<FeedbackViewModel>>(dbModel);
                return model;
            }

            return new List<FeedbackViewModel>();
        }

        public async Task<FeedbackViewModel> GetById(int Id)
        {
            var dbModel = await _feedbackRepository.GetById(Id);
            if (dbModel == null)
                return new FeedbackViewModel();

            var model = Mapper.Map<FeedbackViewModel>(dbModel);

            return model;
        }
    }
}
