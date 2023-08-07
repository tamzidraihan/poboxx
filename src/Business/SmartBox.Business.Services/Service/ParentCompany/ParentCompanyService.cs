using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.ParentCompany;
using SmartBox.Business.Core.Models.ParentCompany;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.ParentCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.ParentCompany
{
    public class ParentCompanyService : BaseMessageService<ParentCompanyService>, IParentCompanyService
    {
        protected readonly IParentCompanyRepository _parentCompanyRepository;
        public ParentCompanyService(IAppMessageService appMessageService, IMapper mapper, 
                                    IParentCompanyRepository parentCompanyRepository) : base(appMessageService, mapper)
        {
            _parentCompanyRepository = parentCompanyRepository;
        }


        public async Task<bool> CheckParentCompanyKeyId(string parentCompanyKeyId)
        {
            var isExisting = await _parentCompanyRepository.CheckParentCompanyKeyId(parentCompanyKeyId);
            return isExisting;
        }

        public async Task<bool> CheckParentCompanyName(string parentCompanyName)
        {
            var isExisting = await _parentCompanyRepository.CheckParentCompanyName(parentCompanyName: parentCompanyName);
            return isExisting;
        }

        public ResponseValidityModel ValidateCompany(ParentCompanyPostModel parentCompanyPostModel)
        {
            var model = new ResponseValidityModel();

            if (!parentCompanyPostModel.ParentCompanyName.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.CompanyModel.CompanyName);
            }

            if (!parentCompanyPostModel.ParentCompanyContactNumber.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.CompanyModel.ContactNumber);
            }

            if (!parentCompanyPostModel.ParentCompanyContactPerson.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.CompanyModel.ContactPerson);
            }

            if (!parentCompanyPostModel.ParentCompanyAddress.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.CompanyModel.Address);
            }

            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

            return model;
        }

        public async Task<List<ParentCompanyViewModel>> GetParentCompany(bool isAndOperator = true, string parentCompanyKeyId = null, string parentCompanyName = null)
        {
            var dbModel = await _parentCompanyRepository.GetParentCompany(isAndOperator, parentCompanyKeyId, parentCompanyName);
            var model = Mapper.Map<List<ParentCompanyViewModel>>(dbModel);
            return model;
        }

        public async Task<ResponseValidityModel> SetParentCompany(ParentCompanyPostModel parentCompanyPostModel)
        {
            var model = ValidateCompany(parentCompanyPostModel);
            if (model.MessageReturnNumber == 0)
            {
                var isInsert = false;
                
                if(!CheckParentCompanyKeyId(parentCompanyPostModel.ParentCompanyKeyId).Result)
                {
                    isInsert = true;
                    var id = await _parentCompanyRepository.GetLastIdentity() + 1;
                    parentCompanyPostModel.ParentCompanyKeyId = Shared.SharedServices.GenerateCompanyKeyId(id, parentCompanyPostModel.ParentCompanyName,true);
                }
              
                var entity = Mapper.Map<ParentCompanyEntity>(parentCompanyPostModel);
                var ret = await _parentCompanyRepository.SaveParentCompany(entity, isInsert);
                
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }

            return model;
        }
    }
}
