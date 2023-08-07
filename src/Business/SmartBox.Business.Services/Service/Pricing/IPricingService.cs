using SmartBox.Business.Core.Models.Pricing;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Pricing
{
    public interface IPricingService
    {
        Task<ResponseValidityModel> SavePricingType(PricingTypeModel type);
        Task<List<PricingTypeModel>> GetPricingType();
        Task<ResponseValidityModel> SavePricingMatrixConfig(PriceMatrixConfigModel priceMatrix);
        Task<List<PriceMatrixConfigModel>> GetPricingMatrixConfig(int? PricingTypeId = null, int? selectedId = null, short? isActive=null);
        Task<ResponseValidityModel> SavePriceAndCharging(PriceAndChargingModel param);
        Task<List<PriceAndChargingModel>> GetPriceAndCharging();
        Task<ResponseValidityModel> DeletePricingType(int id);
        Task<ResponseValidityModel> DeletePriceMatrixConfig(int id);
        Task<ResponseValidityModel> DeletePriceAndCharging(int id);
        Task<ResponseValidityModel> ActivateDeactivate(int Id, int? isActive);

    }
}
