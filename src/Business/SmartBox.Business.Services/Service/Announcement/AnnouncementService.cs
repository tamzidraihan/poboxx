using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Announcement;
using SmartBox.Business.Core.Models.Announcement;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Infrastructure.Data.Repository.Announcement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Announcement
{
    public class AnnouncementService : BaseMessageService<AnnouncementService>, IAnnouncementService
    {
        private readonly IPromoAnnouncementRepository promoAnnouncementRepository;
        private readonly IAnnouncementTypeRepository announcementTypeRepository;
        public AnnouncementService(IAppMessageService appMessageService, IMapper mapper,
            IPromoAnnouncementRepository promoAnnouncementRepository,
            IAnnouncementTypeRepository announcementTypeRepository
            ) : base(appMessageService, mapper)
        {
            this.promoAnnouncementRepository = promoAnnouncementRepository;
            this.announcementTypeRepository = announcementTypeRepository;
        }
        public async Task<ResponseValidityModel> SaveAnnouncementType(AnnouncementTypeModel type)
        {
            var model = ValidateAnnouncementType(type);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<AnnouncementTypeModel, AnnouncementTypeEntity>(type);
                var ret = await announcementTypeRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidateAnnouncementType(AnnouncementTypeModel type)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(type.Name))
                model.MessageReturnNumber = 1;

            return model;
        }
        public async Task<List<AnnouncementTypeModel>> GetAnnouncementTypes()
        {
            var dbModel = await announcementTypeRepository.Get();
            var model = new List<AnnouncementTypeModel>();

            if (dbModel != null)
                model = Mapper.Map<List<AnnouncementTypeModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> SavePromoAnnouncement(PromoAnnouncementModel promoAnnouncement)
        {
            var model = ValidatePromoAnnouncement(promoAnnouncement);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<PromoAnnouncementModel, PromoAnnouncementEntity>(promoAnnouncement);
                var ret = await promoAnnouncementRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidatePromoAnnouncement(PromoAnnouncementModel promoAnnouncement)
        {
            var model = new ResponseValidityModel();
            if (promoAnnouncement.AnnouncementTypeId < 1 || string.IsNullOrEmpty(promoAnnouncement.Name))
                model.MessageReturnNumber = 1;
            return model;
        }
        public async Task<List<PromoAnnouncementModel>> GetPromoAnnouncements()
        {
            var dbModel = await promoAnnouncementRepository.Get();
            var model = new List<PromoAnnouncementModel>();

            if (dbModel != null)
                model = Mapper.Map<List<PromoAnnouncementModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> DeleteAnnouncementType(int id)
        {
            var model = new ResponseValidityModel();
            if (id > 0)
            {
                var ret = await announcementTypeRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
        public async Task<ResponseValidityModel> DeletePromoAnnouncement(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await promoAnnouncementRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
    }
}
