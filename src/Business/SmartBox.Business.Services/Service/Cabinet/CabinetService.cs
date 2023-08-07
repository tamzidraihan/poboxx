using AutoMapper;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Location;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.Cabinet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Slapper.AutoMapper;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.Cabinet
{
    public class CabinetService : BaseMessageService<CabinetService>, ICabinetService
    {
        private readonly ICabinetRepository _cabinetRepository;
        private readonly ILocationService _locationService;
        public CabinetService(IAppMessageService appMessageService, IMapper mapper, ICabinetRepository cabinetRepository, ILocationService locationService) : base(appMessageService, mapper)
        {
            _cabinetRepository = cabinetRepository;
            _locationService = locationService;
        }

        ResponseValidityModel ValidateCabinet(CabinetModel cabinet)
        {
            var model = new ResponseValidityModel();
            model.MessageReturnNumber = 0;
            if (cabinet.CabinetId == 0)
            {
                var isExist = GetCabinet().Result.Where(c => c.CabinetLocationId == cabinet.CabinetLocationId &&
                                                            c.CabinetNumber == cabinet.CabinetNumber).FirstOrDefault();

                if (isExist != null)
                    model = AppMessageService.SetMessage(GlobalConstants.ApplicationMessageNumber.ErrorMessage.CabinetLocationAlreadyExists).MappedResponseValidityModel();
            }


            return model;
        }

        ResponseValidityModel  ValidateCabinetLocation(CabinetLocationModel cabinetLocation)
        {
            var model = new ResponseValidityModel();
            model.MessageReturnNumber = 0;
            if (cabinetLocation.CabinetLocationId == 0)
            {
                var existing = _cabinetRepository.GetCabinetLocation().Result.Where(s => s.Address == cabinetLocation.Address && s.Description == cabinetLocation.Description).ToList();

                if (existing.Count > 0)
                    model = AppMessageService.SetMessage(GlobalConstants.ApplicationMessageNumber.ErrorMessage.CabinetLocationAlreadyExists).MappedResponseValidityModel();
            }
                return model;
        }

        ResponseValidityModel ValidateLockerType(LockerTypeModel cabinetType)
        {
            var model = new ResponseValidityModel();

            var type = this.GetLockerType().Result.Where(r => r.Description == cabinetType.Description).FirstOrDefault();

            if (type != null)
                model = AppMessageService.SetMessage(GlobalConstants.ApplicationMessageNumber.ErrorMessage.LockerTypeAlreadyExists).MappedResponseValidityModel();

            return model;
        }

        async Task MappedLocation(List<CabinetLocationViewModel> model)
        {
            foreach (var item in model)
            {
                var region = await _locationService.GetRegion(item.RegionId);
                if (region != null)
                {
                    item.RegionName = region.region_name;
                }

                var province = await _locationService.GetProvince(item.ProvinceId);
                if (province != null)
                {
                    item.ProvinceName = province.province_name;
                }

                var city = await _locationService.GetCity(item.CityId);
                if (city != null)
                {
                    item.CityName = city.city_name;
                }

                var barangay = await _locationService.GetBarangay(item.BarangayId);
                if (barangay != null)
                {
                    item.BarangayName = barangay.brgyDesc;
                }
            }
        }


        public async Task<ResponseValidityModel> SaveCabinet(CabinetModel cabinet)
        {
            var model = ValidateCabinet(cabinet);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<CabinetModel, CabinetEntity>(cabinet);
                var ret = await _cabinetRepository.SetCabinetEntity(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }

        public async Task<ResponseValidityModel> SaveCabinetLocation(CabinetLocationModel cabinetLocation)
        {
            var model =  ValidateCabinetLocation(cabinetLocation);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<CabinetLocationModel, CabinetLocationEntity>(cabinetLocation);
                var ret = await _cabinetRepository.SetCabinetLocationEntity(entity);

                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }

            return model;
        }

        public async Task<ResponseValidityModel> SaveLockerType(LockerTypeModel lockerType)
        {
            var model = ValidateLockerType(lockerType);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<LockerTypeModel, LockerTypeEntity>(lockerType);
                var ret = await _cabinetRepository.SaveLockerType(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }

        public Task<ResponseValidityModel> UpdateCabinet(CabinetModel cabinet)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseValidityModel> UpdateCabinetLocation(CabinetLocationModel cabinetLocation)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseValidityModel> UpdateLockerType(LockerTypeModel cabinetType)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CabinetLocationViewModel>> GetCabinetLocation()
        {
            var dbModel = await _cabinetRepository.GetCabinetLocation();
            var model = new List<CabinetLocationViewModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetLocationViewModel>>(dbModel);
            }
            return model;
        }

        public async Task<List<CabinetLocationViewModel>> GetCabinetLocationByCompany(int? companyId)
        {
            var dbModel = await _cabinetRepository.GetCabinetLocation();
            List<CabinetLocationEntity> filtered = new List<CabinetLocationEntity>();
            if (companyId == null || companyId == 0)
                filtered = dbModel.Where(o => o.CompanyId == null).ToList();
            else
                filtered = dbModel.Where(o => o.CompanyId == null || o.CompanyId == companyId).ToList();

            var model = new List<CabinetLocationViewModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetLocationViewModel>>(filtered);
            }
            return model;
        }

        public async Task<List<CabinetLocationViewModel>> GetActiveCabinetWithLocation(bool IsMapRegion)
        {
            var dbModel = await _cabinetRepository.GetActiveCabinetLocation();
            var model = new List<CabinetLocationViewModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetLocationViewModel>>(dbModel);
                if (IsMapRegion)
                    await MappedLocation(model);
            }

            //if (dbModel != null)
            //{
            //    model = Mapper.Map<List<CabinetLocationViewModel>>(dbModel);
            //    foreach (var item in model)
            //    {
            //        var region = await _locationService.GetRegion(item.RegionId);
            //        if (region != null)
            //        {
            //            item.RegionName = region.region_name;
            //        }

            //        var province = await _locationService.GetProvince(item.ProvinceId);
            //        if (province != null)
            //        {
            //            item.ProvinceName = province.province_name;
            //        }

            //        var city = await _locationService.GetCity(item.CityId);
            //        if (city != null)
            //        {
            //            item.CityName = city.city_name;
            //        }

            //        var barangay = await _locationService.GetBarangay(item.BarangayId);
            //        if (barangay != null)
            //        {
            //            item.BarangayName = barangay.brgyDesc;
            //        }
            //    }
            //}

            return model;
        }

        public async Task<List<CabinetLocationViewModel>> GetAvailableCabinetLocation()
        {
            var dbModel = await _cabinetRepository.GetAvailableCabinetLocation();
            var model = new List<CabinetLocationViewModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetLocationViewModel>>(dbModel);
                await MappedLocation(model);
            }
            return model;
        }

        public async Task<List<LockerTypeViewModel>> GetLockerType(int? lockerTypeId = null, int? isDeleted=null)
        {
            var dbModel = await _cabinetRepository.GetLockerType(lockerTypeId,isDeleted);
            var model = new List<LockerTypeViewModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<LockerTypeViewModel>>(dbModel);
            }
            return model;
        }

        public async Task<List<CabinetModel>> GetCabinet(int? cabinetid = null, int? companyId = null)
        {
            var dbModel = await _cabinetRepository.GetCabinet(cabinetid, companyId);
            var model = new List<CabinetModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetModel>>(dbModel);
            }
            return model;
        }

        public async Task<List<CabinetModel>> GetCabinetTest(int? cabinetid = null)
        {
            var dbModel = await _cabinetRepository.GetCabinetTest(cabinetid);
            var model = new List<CabinetModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetModel>>(dbModel);
            }
            return model;
        }

        public async Task<List<CabinetViewModel>> GetCabinetWithLocation(bool IsMapRegion)
        {
            var dbModel = await _cabinetRepository.GetCabinet();
            var model = new List<CabinetViewModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetViewModel>>(dbModel);
                //await MappedLocation(model);
                if (IsMapRegion)
                {
                    foreach (var item in model)
                    {
                        var region = await _locationService.GetRegion(item.RegionId);
                        if (region != null)
                        {
                            item.RegionName = region.region_name;
                        }

                        var province = await _locationService.GetProvince(item.ProvinceId);
                        if (province != null)
                        {
                            item.ProvinceName = province.province_name;
                        }

                        var city = await _locationService.GetCity(item.CityId);
                        if (city != null)
                        {
                            item.CityName = city.city_name;
                        }

                        var barangay = await _locationService.GetBarangay(item.BarangayId);
                        if (barangay != null)
                        {
                            item.BarangayName = barangay.brgyDesc;
                        }
                    }
                }


            }

            return model;
        }

        public async Task<ResponseValidityModel> SetLockerTypeActivation(int id, bool isDeleted)
        {

            var ret = await _cabinetRepository.SetLockerTypeActivation(id, isDeleted);
            return AppMessageService.SetMessage(ret).MappedResponseValidityModel();
        }

        public async Task<ResponseValidityModel> SetCabinetLocationActivation(int id, bool isDeleted)
        {
            var ret = await _cabinetRepository.SetCabinetLocationActivation(id, isDeleted);
            return AppMessageService.SetMessage(ret).MappedResponseValidityModel();
        }

        public async Task<ResponseValidityModel> SetCabinetActivation(int id, bool isDeleted)
        {
            var ret = await _cabinetRepository.SetCabinetActivation(id, isDeleted);
            return AppMessageService.SetMessage(ret).MappedResponseValidityModel();
        }

        public async Task<List<LockerTypeViewModel>> GetActiveLockerType()
        {
            var dbModel = await _cabinetRepository.GetActiveLockerType();
            var model = new List<LockerTypeViewModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<LockerTypeViewModel>>(dbModel);
            }

            return model;
        }

        public async Task<List<CabinetModel>> GetActiveCabinet(int? companyId = null)
        {
            var dbModel = await _cabinetRepository.GetActiveCabinet(companyId);
            var model = new List<CabinetModel>();

            if (dbModel != null)
            {
                model = Mapper.Map<List<CabinetModel>>(dbModel);
            }

            return model;
        }


        public async Task<ResponseValidityModel> SaveCabinetType(CabinetTypeViewModel cabinetType)
        {
            var model = await ValidateCabinetType(cabinetType);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<CabinetTypeViewModel, CabinetTypeEntity>(cabinetType);
                var ret = await _cabinetRepository.SaveCabinetType(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        async Task<ResponseValidityModel> ValidateCabinetType(CabinetTypeViewModel cabinet)
        {
            var model = new ResponseValidityModel();
            if (string.IsNullOrEmpty(cabinet.Name))
            {
                model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidCustomField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(MessageParameters.Field, "Name");
            }

            else
            {
                var existing = await _cabinetRepository.GetCabinetTypes(cabinet.Name, cabinet.CabinetTypeId > 0 ? cabinet.CabinetTypeId : null);
                if (existing.Count > 0)
                {
                    model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldExisting).MappedResponseValidityModel();
                    model.Message = model.Message.Replace(MessageParameters.Field, "Name");
                }
            }

            return model;
        }
        public async Task<List<CabinetTypeViewModel>> GetCabinetTypes()
        {
            var dbModel = await _cabinetRepository.GetCabinetTypes();
            var model = new List<CabinetTypeViewModel>();
            
            model = Mapper.Map<List<CabinetTypeViewModel>>(dbModel);

           
            return model;
        }
        public async Task<ResponseValidityModel> DeleteCabinetType(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await _cabinetRepository.DeleteCabinetType(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.Message = "Invalid CabinetTypeId";

            return model;
        }
        public async Task<ResponseValidityModel> ActivateDeactivateCabinetType(int cabinetTypeId, bool isActivated)
        {
            var model = new ResponseValidityModel();
            short iStatus = 0;
            iStatus= isActivated == false ? (short)1 : (short)0;
            if (cabinetTypeId > 0)
            {
                var ret = await _cabinetRepository.ActivateDeactivateCabinetType(cabinetTypeId, iStatus);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
            {
                model.Message = "Invalid CabinetTypeId";
                model.MessageReturnNumber = -600;
            }

            return model;
        }

    }
}
