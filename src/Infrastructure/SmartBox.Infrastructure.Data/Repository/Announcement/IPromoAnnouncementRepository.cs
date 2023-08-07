using SmartBox.Business.Core.Entities.Announcement;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Announcement
{
    public interface IPromoAnnouncementRepository : IGenericRepositoryBase<PromoAnnouncementEntity>
    {
        Task<List<PromoAnnouncementEntity>> Get();
        Task<int> Save(PromoAnnouncementEntity model);
        Task<int> Delete(int id);
    }
}
