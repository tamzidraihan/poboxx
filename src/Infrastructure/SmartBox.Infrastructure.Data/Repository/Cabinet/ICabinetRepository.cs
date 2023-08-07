using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Cabinet
{
    public interface ICabinetRepository: IGenericRepositoryBase<CabinetEntity>
    {
        Task<int> SaveLockerType(LockerTypeEntity lockerTypeEntity);
        //Task<int> UpdateCabinetTypeEntity(LockerTypeEntity lockerTypeEntity);
        Task<int> SetCabinetLocationEntity(CabinetLocationEntity cabinetLocationEntity);
        Task<int> SetCabinetEntity(CabinetEntity cabinetEntity);
        Task<int> SetLockerTypeActivation(int id, bool isDeleted);
        Task<int> SetCabinetLocationActivation(int id, bool isDeleted);
        Task<int> SetCabinetActivation(int id, bool isDeleted);
        Task<List<CabinetLocationEntity>> GetCabinetLocation();
        Task<List<CabinetLocationEntity>> GetAvailableCabinetByLocation(string description, int? skipCabinetLocationId = null);
        Task<List<CabinetLocationEntity>> GetActiveCabinetLocation();
        Task<List<CabinetLocationEntity>> GetAvailableCabinetLocation();
        Task<List<LockerTypeEntity>> GetLockerType(int? lockerTypeId = null,int?isDeleted=null);
        Task<List<LockerTypeEntity>> GetActiveLockerType();
        Task<List<CabinetEntity>> GetCabinet(int? cabinetid = null, int? companyId = null);
        Task<List<CabinetEntity>> GetCabinetTest(int? cabinetid = null);
        Task<List<CabinetEntity>> GetActiveCabinet(int? companyId = null);
        Task<CabinetLocationEntity> CheckCabinetLocationId(int cabinetLocationId);
        Task<CabinetLocationEntity> CheckIfExistCabinetLocationId(int cabinetLocationId);
        Task<int> SetCompanyCabinetLocation(int companyId, List<int> cabinetLocationIds);

        Task<int> SaveCabinetType(CabinetTypeEntity cabinetTypesEntity);
        Task<List<CabinetTypeEntity>> GetCabinetTypes(string existingName = null, int? ignoreCabinetTypeId = null);
        Task ProcessingCompanyCabinets(List<int> cabinetLocationIds, int companyId);
        Task<int> DeleteCompanyCabinets(int companyId);
        Task<int> DeleteCabinetType(int cabinetTypeId);
        Task<int> ActivateDeactivateCabinetType(int cabinetTypeId, short isDeactivate);
    }
}
