using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Cabinet
{
    public interface ICabinetService
    {
        Task<ResponseValidityModel> SaveLockerType(LockerTypeModel lockerType);
        Task<ResponseValidityModel> UpdateLockerType(LockerTypeModel lockerType);
        Task<ResponseValidityModel> SaveCabinetLocation(CabinetLocationModel cabinetLocation);
        Task<ResponseValidityModel> UpdateCabinetLocation(CabinetLocationModel cabinetLocation);
        Task<ResponseValidityModel> SaveCabinet(CabinetModel cabinet);
        Task<ResponseValidityModel> UpdateCabinet(CabinetModel cabinet);
        Task<ResponseValidityModel> SetLockerTypeActivation(int id, bool isDeleted);
        Task<ResponseValidityModel> SetCabinetLocationActivation(int id, bool isDeleted);
        Task<ResponseValidityModel> SetCabinetActivation(int id, bool isDeleted);
        Task<List<CabinetLocationViewModel>> GetCabinetLocation();
        Task<List<CabinetLocationViewModel>> GetCabinetLocationByCompany(int? companyId);
        Task<List<CabinetLocationViewModel>> GetActiveCabinetWithLocation(bool IsMapRegion);
        Task<List<LockerTypeViewModel>> GetLockerType(int? lockerTypeId = null, int?isDeleted = null);
        Task<List<LockerTypeViewModel>> GetActiveLockerType();
        Task<List<CabinetModel>> GetCabinet(int? cabinetid = null,int? companyId = null);
        Task<List<CabinetModel>> GetCabinetTest(int? cabinetid = null);
        Task<List<CabinetModel>> GetActiveCabinet(int? companyId = null);
        Task<List<CabinetViewModel>> GetCabinetWithLocation(bool IsMapRegion);

        Task<List<CabinetTypeViewModel>> GetCabinetTypes();
        Task<ResponseValidityModel> SaveCabinetType(CabinetTypeViewModel cabinetType);
        Task<ResponseValidityModel> DeleteCabinetType(int id);
        Task<ResponseValidityModel> ActivateDeactivateCabinetType(int cabinetTypeId, bool isActivated);


    }
}
