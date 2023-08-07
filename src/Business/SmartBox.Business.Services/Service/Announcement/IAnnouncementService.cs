using SmartBox.Business.Core.Models.Announcement;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Announcement
{
    public interface IAnnouncementService
    {
        Task<ResponseValidityModel> SaveAnnouncementType(AnnouncementTypeModel type);
        Task<List<AnnouncementTypeModel>> GetAnnouncementTypes();
        Task<ResponseValidityModel> SavePromoAnnouncement(PromoAnnouncementModel promoAnnouncement);
        Task<List<PromoAnnouncementModel>> GetPromoAnnouncements();
        Task<ResponseValidityModel> DeleteAnnouncementType(int id);
        Task<ResponseValidityModel> DeletePromoAnnouncement(int id);
    }
}
