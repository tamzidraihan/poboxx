using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Entities.PromoAndDiscounts;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.PromoAndDiscounts;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Feedback;
using SmartBox.Infrastructure.Data.Repository.Feedback;
using SmartBox.Infrastructure.Data.Repository.PromoAndDiscounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.PromoAndDiscounts
{
    public class PromoAndDiscountsService : BaseMessageService<PromoAndDiscountsService>, IPromoAndDiscountsService
    {
        private readonly IPromoAndDiscountsRepository  _promoAndDiscountsRepository;
        public PromoAndDiscountsService(IAppMessageService appMessageService, IMapper mapper, IPromoAndDiscountsRepository promoAndDiscountsRepository) : base(appMessageService, mapper)
        {
            _promoAndDiscountsRepository = promoAndDiscountsRepository;
        }


        ResponseValidityModel ValidatePromoAndDiscounts(PromoAndDiscountsModel promoAndDiscountsModel)
        {
            var model = new ResponseValidityModel();


            return model;
        }

        public async Task<ResponseValidityModel> Save(PromoAndDiscountsModel model)
        {
            var dataModel = ValidatePromoAndDiscounts(model);

            if (dataModel.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<PromoAndDiscountsModel, PromoAndDiscountsEntity>(model);
                var ret = await _promoAndDiscountsRepository.Save(entity);
                dataModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

            }

            return dataModel;
        }


        public async Task<ResponseValidityModel> Delete(int Id)
        {
            var model = new ResponseValidityModel();

            if (Id > 0)
            {
                var ret = await _promoAndDiscountsRepository.DeletePromoAndDiscounts(Id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public List<PromoAndDiscountsModel> GetAll()
        {
            var dbModel = _promoAndDiscountsRepository.GetPromoAndDiscounts();
            if (dbModel != null)
            {
                var model = Mapper.Map<List<PromoAndDiscountsModel>>(dbModel);
                return model;
            }

            return new List<PromoAndDiscountsModel>();
        }

        public async Task<PromoAndDiscountsModel> GetById(int Id)
        {
            var dbModel = await _promoAndDiscountsRepository.GetById(Id);
            if (dbModel == null)
                return new PromoAndDiscountsModel();

            var model = Mapper.Map<PromoAndDiscountsModel>(dbModel);

            return model;
        }
    }
}
