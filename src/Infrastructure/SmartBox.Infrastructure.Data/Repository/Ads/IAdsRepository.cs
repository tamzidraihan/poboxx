using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Entities.PromoAndDiscounts;
using SmartBox.Business.Core.Models.Ads;
using SmartBox.Business.Core.Models.PromoAndDiscounts;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Ads
{
    public interface IAdsRepository : IGenericRepositoryBase<AdsEntity>
    {

        Task<int> Save(AdsEntity feedback);
        Task<AdsEntity> GetById(int Id);

        List<AdsViewModel> GetAds();

        Task<int> DeleteAds(int Id);
    }

}
