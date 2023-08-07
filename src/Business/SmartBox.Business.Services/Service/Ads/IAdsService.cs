using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.PromoAndDiscounts;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Ads
{
    public interface IAdsService
    {
        Task<ResponseValidityModel> Save(AdsModel model);
        List<AdsModel> GetAll();
        Task<AdsModel> GetById(int Id);
        Task<ResponseValidityModel> Delete(int Id);
    }
}
