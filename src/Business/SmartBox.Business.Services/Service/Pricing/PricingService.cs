using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Pricing;
using SmartBox.Business.Core.Models.Pricing;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Infrastructure.Data.Repository.Pricing;
using SmartBox.Business.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Slapper.AutoMapper;

namespace SmartBox.Business.Services.Service.Pricing
{
    public class PricingService : BaseMessageService<PricingService>, IPricingService
    {
        private readonly IPriceAndChargingRepository priceAndChargingRepository;
        private readonly IPricingMatrixConfigRepository priceMatrixConfigRepository;
        private readonly IPricingTypeRepository pricingTypeRepository;
        public PricingService(IAppMessageService appMessageService, IMapper mapper,
            IPriceAndChargingRepository priceAndChargingRepository,
            IPricingMatrixConfigRepository priceMatrixConfigRepository,
            IPricingTypeRepository pricingTypeRepository
            ) : base(appMessageService, mapper)
        {
            this.priceAndChargingRepository = priceAndChargingRepository;
            this.priceMatrixConfigRepository = priceMatrixConfigRepository;
            this.pricingTypeRepository = pricingTypeRepository;
        }
        public async Task<ResponseValidityModel> SavePricingType(PricingTypeModel type)
        {
            var model = ValidatePricingType(type);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<PricingTypeModel, PricingTypeEntity>(type);
                var ret = await pricingTypeRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidatePricingType(PricingTypeModel type)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(type.Name))
                model.MessageReturnNumber = 1;

            if (type.Id == 0)
            {
                var pricing = GetPricingType().Result.ToList().Where(e => e.Name == type.Name).FirstOrDefault();

                if (pricing != null)
                    model = AppMessageService.SetMessage(GlobalConstants.ApplicationMessageNumber.ErrorMessage.PricingTypeAlreadyExists).MappedResponseValidityModel();
            }
            return model;
        }
        public async Task<List<PricingTypeModel>> GetPricingType()
        {
            var dbModel = await pricingTypeRepository.Get();
            var model = new List<PricingTypeModel>();

            if (dbModel != null)
                model = Mapper.Map<List<PricingTypeModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> SavePricingMatrixConfig(PriceMatrixConfigModel priceMatrix)
        {
            var model = await ValidatePricingMatrixConfig(priceMatrix);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<PriceMatrixConfigModel, PricingMatrixConfigEntity>(priceMatrix);
                var ret = await priceMatrixConfigRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        async Task<ResponseValidityModel> ValidatePricingMatrixConfig(PriceMatrixConfigModel priceMatrix)
        {
            var model = new ResponseValidityModel();
            if (priceMatrix.PricingTypeId < 1 || priceMatrix.OverstayPeriod < 1)
                model.MessageReturnNumber = 1;

            var matrix = await GetPricingMatrixConfig(priceMatrix.PricingTypeId, priceMatrix.Id);

            if (matrix.Count > 0)
                model = AppMessageService.SetMessage(GlobalConstants.ApplicationMessageNumber.ErrorMessage.PricingConfigMatrixAlreadyExists).MappedResponseValidityModel();


            return model;
        }
        public async Task<List<PriceMatrixConfigModel>> GetPricingMatrixConfig(int? PricingTypeId = null, int? selectedId = null, short? isActive=null)
        {
            var dbModel = await priceMatrixConfigRepository.Get(PricingTypeId, selectedId,isActive);
            var model = new List<PriceMatrixConfigModel>();

            if (dbModel != null)
                model = Mapper.Map<List<PriceMatrixConfigModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> SavePriceAndCharging(PriceAndChargingModel param)
        {
            var model = ValidatePriceAndCharging(param);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<PriceAndChargingModel, PriceAndChargingEntity>(param);
                var ret = await priceAndChargingRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidatePriceAndCharging(PriceAndChargingModel param)
        {
            var model = new ResponseValidityModel();
            if  (param.LockerTypeId < 1 || param.PricingMatrixId < 1
                || param.LocationId < 1 || param.StoragePrice < 1 || param.MultiAccessStoragePrice < 1)
              model.MessageReturnNumber = 1;
             else {
                if (param.Id == 0)
                {
                    var pAndC = GetPriceAndCharging().Result.ToList().Where(e => e.PricingMatrixId == param.PricingMatrixId
                                    && e.LocationId == param.LocationId && e.LockerTypeId == param.LockerTypeId).FirstOrDefault();

                    if (pAndC != null)
                        model = AppMessageService.SetMessage(GlobalConstants.ApplicationMessageNumber.ErrorMessage.PricingConfigMatrixAlreadyExists).MappedResponseValidityModel();
                }
            }
            return model;
        }
        public async Task<List<PriceAndChargingModel>> GetPriceAndCharging()
        {
            var dbModel = await priceAndChargingRepository.Get();
            var model = new List<PriceAndChargingModel>();

            if (dbModel != null)
                model = Mapper.Map<List<PriceAndChargingModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> DeletePricingType(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await pricingTypeRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;

            return model;
        }
        public async Task<ResponseValidityModel> DeletePriceMatrixConfig(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await priceMatrixConfigRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

        public async Task<ResponseValidityModel> DeletePriceAndCharging(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await priceAndChargingRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }

       public async Task<ResponseValidityModel> ActivateDeactivate(int Id, int? isActive)
        {
            var model = new ResponseValidityModel();
      

            if (Id > 0 && (isActive >=0 && isActive <=1))
            {

                var ret = await priceMatrixConfigRepository.ActivateDeactivate(Id, isActive);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
    }

}
