using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Entities.Company;
using SmartBox.Business.Core.Models.Cabinet;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using SmartBox.Infrastructure.Data.Repository.Company;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Cabinet
{
    public class CabinetRepository : GenericRepositoryBase<CabinetEntity, CabinetRepository>, ICabinetRepository
    {
        public CabinetRepository(IDatabaseHelper databaseHelper, ILogger<CabinetRepository> logger) : base(databaseHelper, logger)
        {

        }

        string BuildInsertCabinetEntityScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Cabinets));

            sql.Append(" (");
            sql.Append(nameof(CabinetEntity.CabinetLocationId));
            sql.Append($", {nameof(CabinetEntity.CabinetNumber)}");
            sql.Append($", {nameof(CabinetEntity.NumberOfLocker)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(CabinetEntity.CabinetLocationId)}");
            sql.Append($", @{nameof(CabinetEntity.CabinetNumber)}");
            sql.Append($", @{nameof(CabinetEntity.NumberOfLocker)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCabinet(CabinetEntity cabinetEntity)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.Cabinets, " SET "));
            var hasComma = false;

            if (cabinetEntity.CabinetLocationId != 0)
            {
                sql.Append(string.Concat(" ", nameof(cabinetEntity.CabinetLocationId), " = ", ""
                    , cabinetEntity.CabinetLocationId));

                hasComma = true;
            }

            if (cabinetEntity.CabinetNumber.HasText())
            {
                if (hasComma)
                {
                    sql.Append(string.Concat(", ", nameof(cabinetEntity.CabinetNumber), " = ",
                        "'", cabinetEntity.CabinetNumber + "'"));
                }
                else
                {
                    sql.Append(string.Concat(" ", nameof(cabinetEntity.CabinetNumber), " = ",
                        "'",  cabinetEntity.CabinetNumber + "'"));

                    hasComma = true;
                }
            }

            if (cabinetEntity.NumberOfLocker != 0)
            {
                if (hasComma)
                {
                    sql.Append(string.Concat(", ", nameof(cabinetEntity.NumberOfLocker), " = ", ""
                        , cabinetEntity.NumberOfLocker));
                }
                else
                {
                    sql.Append(string.Concat(" ", nameof(cabinetEntity.NumberOfLocker), " = ",
                        "", cabinetEntity.NumberOfLocker ));

                    hasComma = true;
                }
            }

            if (hasComma)
                sql.Append(string.Concat(" WHERE ", nameof(cabinetEntity.CabinetId), " = ", "", cabinetEntity.CabinetId));

            return sql.ToString();
        }
        string BuildUpdateCabinetLocation(CabinetLocationEntity cabinetLocationEntity)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.CabinetLocation, " SET "));

            sql.Append(string.Concat(" ", nameof(cabinetLocationEntity.Description), " = ",
                                        "@", nameof(cabinetLocationEntity.Description)));


            if (cabinetLocationEntity.RegionId.HasText())
            {
                sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.RegionId), " = ",
                       "@", nameof(CabinetLocationEntity.RegionId)));
            }

            if (cabinetLocationEntity.ProvinceId.HasText())
            {
                sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.ProvinceId), " = ",
                        "@", nameof(CabinetLocationEntity.ProvinceId)));
            }

            if (cabinetLocationEntity.CityId.HasText())
            {
                sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.CityId), " = ",
                       "@", nameof(CabinetLocationEntity.CityId)));
            }

            if (cabinetLocationEntity.BarangayId.HasText())
            {
                sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.BarangayId), " = ",
                       "@", nameof(CabinetLocationEntity.BarangayId)));
            }

            if (cabinetLocationEntity.BarangayId.HasText())
            {
                sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.BarangayId), " = ",
                       "@", nameof(CabinetLocationEntity.BarangayId)));
            }

            sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.Latitude), " = ",
                       "@", nameof(CabinetLocationEntity.Latitude)));

            sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.Longitude), " = ",
                       "@", nameof(CabinetLocationEntity.Longitude)));

            sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.ZipCode), " = ",
           "@", nameof(CabinetLocationEntity.ZipCode)));

            sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.Rating), " = ",
           "@", nameof(CabinetLocationEntity.Rating)));

            sql.Append(string.Concat(", ", nameof(CabinetLocationEntity.Address), " = ",
                   "@", nameof(CabinetLocationEntity.Address)));

            sql.Append(string.Concat(" WHERE ", nameof(CabinetLocationEntity.CabinetLocationId), " = ", "@", nameof(CabinetLocationEntity.CabinetLocationId)));

            return sql.ToString();
        }
        string BuildInsertCabinetLocationEntityScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.CabinetLocation));

            sql.Append(" (");
            sql.Append(nameof(CabinetLocationEntity.Description));
            sql.Append($", {nameof(CabinetLocationEntity.Address)}");
            sql.Append($", {nameof(CabinetLocationEntity.RegionId)}");
            sql.Append($", {nameof(CabinetLocationEntity.ProvinceId)}");
            sql.Append($", {nameof(CabinetLocationEntity.CityId)}");
            sql.Append($", {nameof(CabinetLocationEntity.BarangayId)}");
            sql.Append($", {nameof(CabinetLocationEntity.ZipCode)}");
            sql.Append($", {nameof(CabinetLocationEntity.Longitude)}");
            sql.Append($", {nameof(CabinetLocationEntity.Latitude)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(CabinetLocationEntity.Description)}");
            sql.Append($", @{nameof(CabinetLocationEntity.Address)}");
            sql.Append($", @{nameof(CabinetLocationEntity.RegionId)}");
            sql.Append($", @{nameof(CabinetLocationEntity.ProvinceId)}");
            sql.Append($", @{nameof(CabinetLocationEntity.CityId)}");
            sql.Append($", @{nameof(CabinetLocationEntity.BarangayId)}");
            sql.Append($", @{nameof(CabinetLocationEntity.ZipCode)}");
            sql.Append($", @{nameof(CabinetLocationEntity.Longitude)}");
            sql.Append($", @{nameof(CabinetLocationEntity.Latitude)}");
            sql.Append(")");

            return sql.ToString();
        }
        string BuildInsertLockerTypeScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.LockerType));

            sql.Append(" (");
            sql.Append(nameof(LockerTypeEntity.Description));
            sql.Append($", {nameof(LockerTypeEntity.Size)}");
            sql.Append($", {nameof(LockerTypeEntity.Price)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(LockerTypeEntity.Description)}");
            sql.Append($", @{nameof(LockerTypeEntity.Size)}");
            sql.Append($", @{nameof(LockerTypeEntity.Price)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateLockerType(LockerTypeEntity lockerTypeEntity)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerType, " SET "));


            sql.Append(string.Concat(" ", nameof(lockerTypeEntity.Description), " = ",
                    "@", nameof(lockerTypeEntity.Description)));

            sql.Append(string.Concat(", ", nameof(lockerTypeEntity.Size), " = ",
                    "@", nameof(lockerTypeEntity.Size)));

            sql.Append(string.Concat(", ", nameof(lockerTypeEntity.Price), " = ",
                     "@", nameof(lockerTypeEntity.Price)));

            sql.Append(string.Concat(" WHERE ", nameof(lockerTypeEntity.LockerTypeId), " = ", "@", nameof(lockerTypeEntity.LockerTypeId)));

            return sql.ToString();
        }
        string BuildGetCabinetLocation()
        {
            string sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.CabinetLocation);

            return sql;
        }
        string BuildGetActiveCabinetLocation()
        {
            string sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.ActiveCabinetLocation);

            return sql;
        }
        string BuildGetAvailableCabinetLocation()
        {
            string sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.ActiveCabinetLocation, " WHERE ", nameof(CabinetLocationEntity.CompanyId), " IS NULL");

            return sql;
        }
        string BuildGetLockerType()
        {
            string sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.DatabaseTables.LockerType);

            return sql;
        }
        string BuildGetActiveLockerType()
        {
            string sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.ActiveLockerType);

            return sql;
        }

        SqlBuilder.Template BuildQuery(int? cabinetID = null)
        {
            var builder = new SqlBuilder();
            builder.Select("*");
            //builder.Select("FirstName");
            //builder.Select("LastName");

            DynamicParameters parameters = new DynamicParameters();

            if (cabinetID != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CabinetId, cabinetID, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(CabinetEntity.CabinetId) + " = " + GlobalDatabaseConstants.QueryParameters.CabinetId, parameters);
            }

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.CabinetWithLocation} /**where**/ ");
            return builderTemplate;
        }

        SqlBuilder.Template BuildGetCabinet(int? cabinetID = null)
        {
            var builder = new SqlBuilder();
            builder.Select("*");

            DynamicParameters parameters = new DynamicParameters();

            if (cabinetID != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CabinetId, cabinetID, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(CabinetEntity.CabinetId) + " = " + GlobalDatabaseConstants.QueryParameters.CabinetId, parameters);
            }

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.CabinetWithLocation} /**where**/ ");
            return builderTemplate;
        }

        string BuildGetActiveCabinet(int? companyId = null)
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.ActiveCabinetAndLocation));

            if (companyId.HasValue && companyId.Value != 0)
                sql.Append(string.Concat(" WHERE ", nameof(CabinetEntity.CompanyId), " = ", "@", nameof(CabinetEntity.CompanyId)));

            return sql.ToString();
        }
        string BuildActivateScriptCabinet()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.Cabinets, " SET "));

            sql.Append(string.Concat(" ", nameof(CabinetEntity.IsDeleted), " = ", "@", nameof(CabinetEntity.IsDeleted)));

            sql.Append(string.Concat(" WHERE ", nameof(CabinetEntity.CabinetId), " = ", "@", nameof(CabinetEntity.CabinetId)));

            return sql.ToString();
        }
        string BuildActivateScriptCabinetLocation()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.CabinetLocation, " SET "));

            sql.Append(string.Concat(" ", nameof(CabinetLocationEntity.IsDeleted), " = ", "@", nameof(CabinetLocationEntity.IsDeleted)));

            sql.Append(string.Concat(" WHERE ", nameof(CabinetLocationEntity.CabinetLocationId), " = ", "@", nameof(CabinetLocationEntity.CabinetLocationId)));

            return sql.ToString();
        }
        string BuildActivateScriptLockerType()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerType, " SET "));

            sql.Append(string.Concat(" ", nameof(LockerTypeEntity.IsDeleted), " = ", "@", nameof(LockerTypeEntity.IsDeleted)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerTypeEntity.LockerTypeId), " = ", "@", nameof(LockerTypeEntity.LockerTypeId)));

            return sql.ToString();
        }

        public async Task<List<CabinetLocationEntity>> GetCabinetLocation()
        {
            var sql = BuildGetCabinetLocation();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetLocationEntity>(sql);

                return dbModel.ToList();
            }
        }

        public async Task<List<CabinetLocationEntity>> GetActiveCabinetLocation()
        {
            var sql = BuildGetActiveCabinetLocation();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetLocationEntity>(sql);

                return dbModel.ToList();
            }
        }

        public async Task<List<CabinetLocationEntity>> GetAvailableCabinetLocation()
        {
            var sql = BuildGetAvailableCabinetLocation();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetLocationEntity>(sql);

                return dbModel.ToList();
            }
        }

        public async Task<List<CabinetLocationEntity>> GetAvailableCabinetByLocation(string description, int? skipCabinetLocationId = null)
        {     var p = new DynamicParameters();
            var sql = "Select * from vw_active_cabinet_location where description = @Description";
          
            if (skipCabinetLocationId.HasValue && skipCabinetLocationId.Value > 0){ 
                sql += " AND CabinetLocationId = @CabinetLocationId";
            }
           

        

            p.Add("@Description", description);
            p.Add("@CabinetLocationId", skipCabinetLocationId.Value);
        


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetLocationEntity>(sql,p);

                return dbModel.ToList();
            }
        }

        public async Task<List<LockerTypeEntity>> GetLockerType(int? lockerTypeId = null, int? isDeleted=null)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();

            builder.Select("*");

            if (isDeleted != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.IsActive, lockerTypeId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerTypeEntity.IsDeleted) + " = " + GlobalDatabaseConstants.QueryParameters.IsDeleted, parameters);
            }

            if (lockerTypeId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.LockerTypeId, lockerTypeId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerTypeEntity.LockerTypeId) + " = " + GlobalDatabaseConstants.QueryParameters.LockerTypeId, parameters);
            }

       

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.ActiveLockerType} /**where**/ ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerTypeEntity>(builderTemplate.RawSql, builderTemplate.Parameters);
                //var dbModel = await conn.QueryAsync<LockerTypeEntity>(sql);

                return dbModel.ToList();
            }
        }

        public async Task<List<LockerTypeEntity>> GetActiveLockerType()
        {
            var sql = BuildGetActiveLockerType();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerTypeEntity>(sql);

                return dbModel.ToList();
            }
        }

        public async Task<List<CabinetEntity>> GetCabinet(int? cabinetid = null, int? companyId = null)
        {
            //var builderTemplate = BuildGetCabinet(cabinetid);
            var builder = new SqlBuilder();
            builder.Select("*");

            DynamicParameters parameters = new DynamicParameters();

            if (cabinetid != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CabinetId, cabinetid, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(CabinetEntity.CabinetId) + " = " + GlobalDatabaseConstants.QueryParameters.CabinetId, parameters);
            }

            if (companyId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(CabinetEntity.CompanyId) + " = " + GlobalDatabaseConstants.QueryParameters.CompanyId, parameters);
            }

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.CabinetWithLocation} /**where**/ ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetEntity>(builderTemplate.RawSql, builderTemplate.Parameters);
                return dbModel.ToList();
            }
        }

        public async Task<List<CabinetEntity>> GetCabinetTest(int? cabinetid = null)
        {
            var builderTemplate = BuildQuery(cabinetid);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                //var author = connection.Query<Author>(builderTemplate.RawSql, builderTemplate.Parameters).FirstOrDefault();
                var dbModel = await conn.QueryAsync<CabinetEntity>(builderTemplate.RawSql, builderTemplate.Parameters);

                return dbModel.ToList();
            }
        }

        public async Task<List<CabinetEntity>> GetActiveCabinet(int? companyId = null)
        {
            var sql = BuildGetActiveCabinet(companyId);

            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(CabinetEntity.CompanyId)), companyId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetEntity>(sql, p);

                return dbModel.ToList();
            }
        }
        public async Task<CabinetLocationEntity> CheckIfExistCabinetLocationId(int cabinetLocationId)
        {
            var p = new DynamicParameters();
            var sql = BuildCheckIfExistCabinetLocationIdSql(cabinetLocationId);
       
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetLocationEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        string BuildCheckIfExistCabinetLocationIdSql(int cabinetLocationId){
           var p = new DynamicParameters();
            var sql = BuildGetActiveCabinetLocationSql();
            p.Add(string.Concat("@", nameof(CabinetLocationEntity.CabinetLocationId)), cabinetLocationId);


            return sql.ToString();
        }

    
        public async Task<CabinetLocationEntity> CheckCabinetLocationId(int cabinetLocationId)
        {
            var p = new DynamicParameters();
            var sql = BuildGetActiveCabinetLocation();
            p.Add(string.Concat("@", nameof(CabinetLocationEntity.CabinetLocationId)), cabinetLocationId);

        
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetLocationEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        string BuildGetActiveCabinetLocationSql()
        {
            var sql = BuildGetActiveCabinetLocation();

            sql = string.Concat(sql, " WHERE ", nameof(CabinetLocationEntity.CabinetLocationId), " = ", "@",
                                nameof(CabinetLocationEntity.CabinetLocationId), " AND ", nameof(CabinetLocationEntity.CompanyId), " IS NULL ");

            return sql.ToString();

        }
        public async Task<int> SetCabinetEntity(CabinetEntity cabinetEntity)
        {
            var p = new DynamicParameters();
            string sql;
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(CabinetEntity.CabinetId)), cabinetEntity.CabinetId);
            p.Add(string.Concat("@", nameof(cabinetEntity.CabinetNumber)), cabinetEntity.CabinetNumber);
            p.Add(string.Concat("@", nameof(cabinetEntity.NumberOfLocker)), cabinetEntity.NumberOfLocker);
            p.Add(string.Concat("@", nameof(cabinetEntity.CabinetLocationId)), cabinetEntity.CabinetLocationId);

            if (cabinetEntity.CabinetId == 0)
                sql = BuildInsertCabinetEntityScript();
            else
            {
                sql = BuildUpdateCabinet(cabinetEntity);
                isInsert = false;
            }

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
 

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p);

                    if (ret > 0)
                    {
                        if (isInsert)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                        else
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

        
                    return ret;
                }
                catch (Exception e)
                {
                   
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<int> SetCabinetLocationEntity(CabinetLocationEntity cabinetLocationEntity)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(cabinetLocationEntity.CabinetLocationId)), cabinetLocationEntity.CabinetLocationId);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.RegionId)), cabinetLocationEntity.RegionId);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.ProvinceId)), cabinetLocationEntity.ProvinceId);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.CityId)), cabinetLocationEntity.CityId);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.BarangayId)), cabinetLocationEntity.BarangayId);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.Address)), cabinetLocationEntity.Address);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.ZipCode)), cabinetLocationEntity.ZipCode);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.Description)), cabinetLocationEntity.Description);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.Longitude)), cabinetLocationEntity.Longitude);
            p.Add(string.Concat("@", nameof(cabinetLocationEntity.Latitude)), cabinetLocationEntity.Latitude);

            string sql;
            if (cabinetLocationEntity.CabinetLocationId == 0)
                sql = BuildInsertCabinetLocationEntityScript();
            else
            {
                sql = BuildUpdateCabinetLocation(cabinetLocationEntity);
                isInsert = false;
            }


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        if (isInsert)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                        else
                        {
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                            isInsert = false;
                        }
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<int> SaveLockerType(LockerTypeEntity lockerTypeEntity)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(lockerTypeEntity.LockerTypeId)), lockerTypeEntity.LockerTypeId);
            p.Add(string.Concat("@", nameof(lockerTypeEntity.Description)), lockerTypeEntity.Description);
            p.Add(string.Concat("@", nameof(lockerTypeEntity.Size)), lockerTypeEntity.Size);
            p.Add(string.Concat("@", nameof(lockerTypeEntity.Price)), lockerTypeEntity.Price);

            string sql;
            if (lockerTypeEntity.LockerTypeId == 0)
                sql = BuildInsertLockerTypeScript();
            else
            {
                sql = BuildUpdateLockerType(lockerTypeEntity);
                isInsert = false;
            }

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        if (isInsert)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                        else
                        {
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                            isInsert = false;
                        }
                    }
                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;


                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<int> SetCabinetActivation(int id, bool isDeleted)
        {
            if (ValidateCabinet(id) == false)
                return GlobalConstants.ApplicationMessageNumber.ErrorMessage.CabinetAssignedWithBooking;

            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(CabinetEntity.CabinetId)), id);

            if (isDeleted)
                p.Add(string.Concat("@", nameof(CabinetEntity.IsDeleted)), 1);
            else
                p.Add(string.Concat("@", nameof(CabinetEntity.IsDeleted)), 0);

            sql = BuildActivateScriptCabinet();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);

                    if (ret > 0)
                    {
                        if (isDeleted)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeactivated;
                        else
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordActivated;
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        private bool ValidateCabinetLocation(int cabinetLocationId)
        {
            string sql = @"select cl.* from locker_bookings lb, cabinets c,
                        cabinet_locations cl, locker_detail ld
                        where cl.CabinetLocationId=c.CabinetLocationId and
                        ld.CabinetId = c.CabinetId and
                        lb.LockerDetailId=ld.LockerDetailId and (lb.BookingStatus=1 OR lb.BookingStatus=2)
                        and c.CabinetLocationId = @CabinetLocationId";
            var p = new DynamicParameters();

             p.Add("@CabinetLocationId", cabinetLocationId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = conn.Query<CabinetLocationEntity>(sql,p);

                if (dbModel.Any())
                    return false;
                else
                    return true;

            }

        }

        private bool ValidateCabinet(int cabinetId)
        {
            string sql = @"select cl.* from locker_bookings lb, cabinets c,
                        cabinet_locations cl, locker_detail ld
                        where cl.CabinetLocationId=c.CabinetLocationId and
                        ld.CabinetId = c.CabinetId and
                        lb.LockerDetailId=ld.LockerDetailId and (lb.BookingStatus=1 OR lb.BookingStatus=2)
                        and c.CabinetId= @CabinetId";
            var p = new DynamicParameters();

             p.Add(string.Concat("@CabinetId", cabinetId));

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = conn.Query<CabinetLocationEntity>(sql,p);

                if (dbModel.Any())
                    return false;
                else
                    return true;

            }

        }
        public async Task<int> SetCabinetLocationActivation(int id, bool isDeleted)
        {
            if (ValidateCabinetLocation(id) == false)
                return GlobalConstants.ApplicationMessageNumber.ErrorMessage.CabinetLocationAssignedWithBooking;

            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(CabinetLocationEntity.CabinetLocationId)), id);

            if (isDeleted)
                p.Add(string.Concat("@", nameof(CabinetLocationEntity.IsDeleted)), 1);
            else
                p.Add(string.Concat("@", nameof(CabinetLocationEntity.IsDeleted)), 0);

            sql = BuildActivateScriptCabinetLocation();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);

                    if (ret > 0)
                    {
                        if (isDeleted)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeactivated;
                        else
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordActivated;
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }


        public async Task<int> SetLockerTypeActivation(int id, bool isDeleted)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(LockerTypeEntity.LockerTypeId)), id);

            if (isDeleted)
                p.Add(string.Concat("@", nameof(CabinetEntity.IsDeleted)), 1);
            else
                p.Add(string.Concat("@", nameof(CabinetEntity.IsDeleted)), 0);

            sql = BuildActivateScriptLockerType();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);

                    if (ret > 0)
                    {
                        if (isDeleted)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeactivated;
                        else
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordActivated;
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<int> SetCompanyCabinetLocation(int companyId, List<int> cabinetLocationIds)
        {
            var p = new DynamicParameters();
            string queryValues;
            string sql;

            List<string> columns = new List<string> {
             nameof(CabinetLocationEntity.CompanyId),
            };

            p.Add(string.Concat("@", nameof(CabinetLocationEntity.CompanyId)), companyId);
            p.Add(string.Concat("@", nameof(CabinetLocationEntity.CabinetLocationId)), cabinetLocationIds);

            queryValues = SharedServices.UpdateQueryBuilder(columns);
            sql = $"UPDATE {GlobalDatabaseConstants.DatabaseTables.CabinetLocation} SET {queryValues} " +
                  $"WHERE {nameof(CabinetLocationEntity.CabinetLocationId)} in @{nameof(CabinetLocationEntity.CabinetLocationId)}";

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        //Scheduling job for Process CompanyCabinets
                        if (cabinetLocationIds.Count > 0)
                            JobScheduler.Enqueue<ICabinetRepository>(s => s.ProcessingCompanyCabinets(cabinetLocationIds, companyId));
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }
        public async Task<int> DeleteCompanyCabinets(int companyId)
        {

            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(CabinetLocationEntity.CompanyId)), companyId);
            string sql = BuildDeleteCommandForCompanyCabinets();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        transaction.Commit();
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeleted;
                    }

                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildDeleteCommandForCompanyCabinets()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.CabinetLocation, " SET "));
            sql.Append(string.Concat(" ", nameof(CabinetLocationEntity.CompanyId), "=NULL"));
            sql.Append(string.Concat(" WHERE ", nameof(CabinetLocationEntity.CompanyId), " = ", "@", nameof(CabinetLocationEntity.CompanyId)));
            return sql.ToString();
        }
        public async Task ProcessingCompanyCabinets(List<int> cabinetLocationIds, int companyId)
        {
            //Getting the active cabinets against the Cabinet Locations
            var activeCabinets = await GetActiveCabinet(cabinetLocationIds);
            if (activeCabinets.Count > 0)
                //Inserting the Company Cabinets
                JobScheduler.Enqueue<ICompanyCabinetRepository>(s => s.SaveBulkRecords(activeCabinets.Select(s => s.CabinetId).ToList(), companyId));
        }
        private async Task<List<ActiveCabinetsId>> GetActiveCabinet(List<int> cabinetLocationIds)
        {
            var sql = $"Select {nameof(CabinetEntity.CabinetId)} from {GlobalDatabaseConstants.Views.ActiveCabinetAndLocation} where {nameof(CabinetEntity.CabinetLocationId)} in ({string.Join(",", cabinetLocationIds)})";

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<ActiveCabinetsId>(sql);

                return dbModel.ToList();
            }
        }
        public async Task<int> SaveCabinetType(CabinetTypeEntity cabinetTypeEntity)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(cabinetTypeEntity.CabinetTypeId)), cabinetTypeEntity.CabinetTypeId);
            p.Add(string.Concat("@", nameof(cabinetTypeEntity.Description)), cabinetTypeEntity.Description);
            p.Add(string.Concat("@", nameof(cabinetTypeEntity.Name)), cabinetTypeEntity.Name);
            p.Add(string.Concat("@", nameof(cabinetTypeEntity.IconImage)), cabinetTypeEntity.IconImage);

            string sql;
            if (cabinetTypeEntity.CabinetTypeId == 0)
                sql = BuildInsertCabinetTypeScript();
            else
            {
                sql = BuildUpdateCabinetType(cabinetTypeEntity);
                isInsert = false;
            }

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        if (isInsert)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                        else
                        {
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                            isInsert = false;
                        }
                    }
                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;


                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }

        string BuildInsertCabinetTypeScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.CabinetTypes));

            sql.Append(" (");
            sql.Append(nameof(CabinetTypeEntity.Name));
            sql.Append($", {nameof(CabinetTypeEntity.Description)}");
            sql.Append($", {nameof(CabinetTypeEntity.IconImage)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(CabinetTypeEntity.Name)}");
            sql.Append($", @{nameof(CabinetTypeEntity.Description)}");
            sql.Append($", @{nameof(CabinetTypeEntity.IconImage)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCabinetType(CabinetTypeEntity cabinetTypeEntity)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.CabinetTypes, " SET "));


            sql.Append(string.Concat(" ", nameof(cabinetTypeEntity.Description), " = ",
                    "@", nameof(cabinetTypeEntity.Description)));

            sql.Append(string.Concat(", ", nameof(cabinetTypeEntity.IconImage), " = ",
                    "@", nameof(cabinetTypeEntity.IconImage)));

            sql.Append(string.Concat(", ", nameof(cabinetTypeEntity.Name), " = ",
                     "@", nameof(cabinetTypeEntity.Name)));

            sql.Append(string.Concat(" WHERE ", nameof(cabinetTypeEntity.CabinetTypeId), " = ", "@", nameof(cabinetTypeEntity.CabinetTypeId)));

            return sql.ToString();

        }
        public async Task<List<CabinetTypeEntity>> GetCabinetTypes(string existingName = null, int? ignoreCabinetTypeId = null)
        {

            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.DatabaseTables.CabinetTypes));
            if (existingName.HasText())
                sql.Append(string.Concat(" WHERE Lower(", nameof(CabinetTypeEntity.Name), ")=Lower('", existingName, "')"));
            if (ignoreCabinetTypeId.HasValue)
                sql.Append(string.Concat(" AND ", nameof(CabinetTypeEntity.CabinetTypeId), "<>", ignoreCabinetTypeId.Value));

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CabinetTypeEntity>(sql.ToString());

                return dbModel.ToList();
            }
        }

        public async Task<int> ActivateDeactivateCabinetType(int cabinetTypeId, short isDeactivate)

        {
            var p = new DynamicParameters();
            string sql = "UPDATE " + GlobalDatabaseConstants.DatabaseTables.CabinetTypes + " SET IsDeactivated = @IsDeactivated";
            sql += " WHERE CabinetTypeId = @CabinetTypeId";

            p.Add ("@IsDeactivated",isDeactivate);
            p.Add("@CabinetTypeId", cabinetTypeId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
              
                try
                {
                    var ret = await conn.ExecuteAsync(sql,p);
                    if (ret > 0)
                    {
                     
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                    }

                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                    return ret;
                }
                catch (Exception e)
                {
            
                    _logger.LogError(e.Message);
                    if (e.Message.Contains("Cannot delete or update a parent row: a foreign key constraint fails"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.PermissionDeleteConstraints;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

            public async Task<int> DeleteCabinetType(int cabinetTypeId)
             {

            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(CabinetTypeEntity.CabinetTypeId)), cabinetTypeId);
            string sql = BuildDeleteCommandForCabinetType();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        transaction.Commit();
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeleted;
                    }

                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    if (e.Message.Contains("Cannot delete or update a parent row: a foreign key constraint fails"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.PermissionDeleteConstraints;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildDeleteCommandForCabinetType()
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.CabinetTypes));

            sql.Append(string.Concat(" WHERE ", nameof(CabinetTypeEntity.CabinetTypeId), " = ", "@", nameof(CabinetTypeEntity.CabinetTypeId)));

            return sql.ToString();

        }
    }
}
