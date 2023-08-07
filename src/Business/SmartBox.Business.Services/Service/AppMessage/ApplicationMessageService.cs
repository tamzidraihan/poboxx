using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.AppMessage;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.AppMessage;
using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.Permission;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Feedback;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.AppMessage;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.AppMessage
{
    public class ApplicationMessageService : BaseMessageService<ApplicationMessageService>, IApplicationMessageService
    {
        private readonly IAppMessageRepository _appMessageRepository;
        public ApplicationMessageService(IAppMessageRepository appMessageRepository, IAppMessageService appMessageService, IMapper mapper) : base(appMessageService, mapper)
        {
            
            _appMessageRepository = appMessageRepository;
        }
         
        public async Task<ApplicationMessageModel> GetApplicationMessageById(int Id)
        {
            var dbModel = await _appMessageRepository.GetApplicationMessage(Id);
            if (dbModel == null)
                return new ApplicationMessageModel(); 
            var model = Mapper.Map<ApplicationMessageModel>(dbModel);

            return model;
        }

        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _appMessageRepository.DeleteApplicationMessage(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
        async Task<ResponseValidityModel> ValidateApplicationMessageCreate(ApplicationMessageModel appMsgModel)
        {


            var model = new ResponseValidityModel();
            var isExisting = await _appMessageRepository.GetApplicationMessage(appMsgModel.ApplicationMessageId);

            if (isExisting != null)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.ApplicationMessage);
                return model;
            }

            if (!appMsgModel.Message.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.ApplicationMessage.Message);
            }

             
            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

           

            return model;
        }
        public async Task<ResponseValidityModel> Create(ApplicationMessageModel model)
        {
            var dataModel = await ValidateApplicationMessageCreate(model);

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<ApplicationMessageModel, ApplicationMessageEntity>(model);
                var ret = await _appMessageRepository.Create(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }
        async Task<ResponseValidityModel> ValidateApplicationMessageUpdate(ApplicationMessageModel appMsgModel)
        {


            var model = new ResponseValidityModel();
            var isExisting = await _appMessageRepository.GetApplicationMessage(appMsgModel.ApplicationMessageId);

            if (isExisting == null)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.ApplicationMessageId);
                return model;
            } 

            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField)
                                              .MappedResponseValidityModel(model.MessagesList);
            }



            return model;
        }
        public async Task<ResponseValidityModel> Update(ApplicationMessageModel model)
        {
            var dataModel = await ValidateApplicationMessageUpdate(model);

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<ApplicationMessageModel, ApplicationMessageEntity>(model);
                var ret = await _appMessageRepository.Update(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }


        public async Task<List<ApplicationMessageModel>> GetAll()
        {
            var dbModel = await _appMessageRepository.GetAll();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<ApplicationMessageModel>>(dbModel);
                return model;
            }

            return new List<ApplicationMessageModel>();
        }
    }
}
