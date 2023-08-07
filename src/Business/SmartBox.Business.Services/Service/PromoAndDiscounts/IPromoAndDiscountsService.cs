using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.PromoAndDiscounts;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.PromoAndDiscounts
{
    public interface IPromoAndDiscountsService
    {
        Task<ResponseValidityModel> Save(PromoAndDiscountsModel model);
        List<PromoAndDiscountsModel> GetAll();
        Task<PromoAndDiscountsModel> GetById(int Id);
        Task<ResponseValidityModel> Delete(int Id);
    }
}
