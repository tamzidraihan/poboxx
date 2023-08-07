using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Feedback;
using SmartBox.Infrastructure.Data.Repository.Ads;
using SmartBox.Infrastructure.Data.Repository.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Ads
{
    public class AdsService : BaseMessageService<AdsService>, IAdsService
    {
        private readonly IAdsRepository _adsRepository;
        public AdsService(IAppMessageService appMessageService, IMapper mapper, IAdsRepository adsRepository) : base(appMessageService, mapper)
        {
            _adsRepository = adsRepository;
        }


        ResponseValidityModel ValidateAds(AdsModel adsModel)
        {
            var model = new ResponseValidityModel();


            return model;
        }

        public async Task<ResponseValidityModel> Save(AdsModel model)
        {
            var dataModel = ValidateAds(model);

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<AdsModel, AdsEntity>(model);
                var ret = await _adsRepository.Save(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }


        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _adsRepository.DeleteAds(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public List<AdsModel> GetAll()
        {
            var dbModel = _adsRepository.GetAds();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<AdsModel>>(dbModel);
                return model;
            }

            return new List<AdsModel>();
        }

        public async Task<AdsModel> GetById(int Id)
        {
            var dbModel = await _adsRepository.GetById(Id);
            if (dbModel == null)
                return new AdsModel();

            var model = Mapper.Map<AdsModel>(dbModel);

            return model;
        }
    }
}
