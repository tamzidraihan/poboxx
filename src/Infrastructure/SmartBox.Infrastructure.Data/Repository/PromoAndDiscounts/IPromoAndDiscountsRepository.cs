using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Entities.PromoAndDiscounts;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.PromoAndDiscounts;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.PromoAndDiscounts
{
    public interface IPromoAndDiscountsRepository : IGenericRepositoryBase<PromoAndDiscountsEntity>
    {

        Task<int> Save(PromoAndDiscountsEntity feedback);
        Task<PromoAndDiscountsEntity> GetById(int Id);

        List<PromoAndDiscountsViewModel> GetPromoAndDiscounts();

        Task<int> DeletePromoAndDiscounts(int Id);
    }
}
