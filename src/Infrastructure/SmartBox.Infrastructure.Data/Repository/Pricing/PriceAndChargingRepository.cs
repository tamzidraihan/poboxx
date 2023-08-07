using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Pricing;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Pricing
{
    public class PriceAndChargingRepository : GenericRepositoryBase<PriceAndChargingEntity, PriceAndChargingRepository>, IPriceAndChargingRepository
    {
        public PriceAndChargingRepository(IDatabaseHelper databaseHelper, ILogger<PriceAndChargingRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<PriceAndChargingEntity>> Get(int? lockerTypeId = null, int? cabinetLocationId = null)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(PriceAndChargingEntity.LockerTypeId)), lockerTypeId);
            p.Add(string.Concat("@", nameof(PriceAndChargingEntity.LocationId)), cabinetLocationId);
           
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<PriceAndChargingEntity>(BuildGetCommand(lockerTypeId, cabinetLocationId), p)).ToList();
            }
        }
        string BuildDeleteCommand(PriceAndChargingEntity model)
        {
            var sql = new StringBuilder("DELETE from overstay_price_configuration") ;
                ;

            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();

        }
        public async Task<int> Delete(int id)
        {
            PriceAndChargingEntity entity = new PriceAndChargingEntity { Id = id };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.Id)), id);
            string sql = BuildDeleteCommand(entity);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeleted;
                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;


                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    if (e.Message.Contains("Cannot delete or update a parent row: a foreign key constraint fails"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.DeleteConstraintsError;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }

        public async Task<int> Save(PriceAndChargingEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.LocationId)), model.LocationId);
            p.Add(string.Concat("@", nameof(model.LockerTypeId)), model.LockerTypeId);
            p.Add(string.Concat("@", nameof(model.OverstayCharge)), model.OverstayCharge);
            p.Add(string.Concat("@", nameof(model.StoragePrice)), model.StoragePrice);
            p.Add(string.Concat("@", nameof(model.MultiAccessStoragePrice)), model.MultiAccessStoragePrice);
            p.Add(string.Concat("@", nameof(model.PricingMatrixId)), model.PricingMatrixId);

            string sql;
            if (model.Id == 0)
            {
                p.Add(string.Concat("@", nameof(model.DateCreated)), DateTime.Now);
                sql = BuildInsertCommand();
            }
            else
            {
                p.Add(string.Concat("@", nameof(model.DateModified)), DateTime.Now);
                sql = BuildUpdateCommand(model);
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
                    if (!string.IsNullOrEmpty(e.Message) && e.Message.Contains("foreign key"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.InvalidForeignId;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildGetCommand(int? lockerTypeId = null, int? cabinetLocationId = null)
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.OverstayPriceConfiguration));

            if (lockerTypeId.HasValue)
            {
                sql.Append(string.Concat(" WHERE ", nameof(PriceAndChargingEntity.LockerTypeId), " = ", "@", nameof(PriceAndChargingEntity.LockerTypeId)));
                if (cabinetLocationId.HasValue)
                    sql.Append(string.Concat(" AND ", nameof(PriceAndChargingEntity.LocationId), " = ", "@", nameof(PriceAndChargingEntity.LocationId)));
            }
            else if (cabinetLocationId.HasValue)
            {
                sql.Append(string.Concat(" WHERE ", nameof(PriceAndChargingEntity.LocationId), " = ", "@", nameof(PriceAndChargingEntity.LocationId)));
                if (lockerTypeId.HasValue)
                    sql.Append(string.Concat(" AND ", nameof(PriceAndChargingEntity.LockerTypeId), " = ", "@", nameof(PriceAndChargingEntity.LockerTypeId)));
            }
            return sql.ToString();
        }

        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.OverstayPriceConfiguration));

            sql.Append(" (");
            sql.Append(nameof(PriceAndChargingEntity.StoragePrice));
            sql.Append($", {nameof(PriceAndChargingEntity.MultiAccessStoragePrice)}");
            sql.Append($", {nameof(PriceAndChargingEntity.OverstayCharge)}");
            sql.Append($", {nameof(PriceAndChargingEntity.LocationId)}");
            sql.Append($", {nameof(PriceAndChargingEntity.LockerTypeId)}");
            sql.Append($", {nameof(PriceAndChargingEntity.PricingMatrixId)}");
            sql.Append($", {nameof(PricingMatrixConfigEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(PriceAndChargingEntity.StoragePrice)}");
            sql.Append($", @{nameof(PriceAndChargingEntity.MultiAccessStoragePrice)}");
            sql.Append($", @{nameof(PriceAndChargingEntity.OverstayCharge)}");
            sql.Append($", @{nameof(PriceAndChargingEntity.LocationId)}");
            sql.Append($", @{nameof(PriceAndChargingEntity.LockerTypeId)}");
            sql.Append($", @{nameof(PriceAndChargingEntity.PricingMatrixId)}");
            sql.Append($", @{nameof(PricingMatrixConfigEntity.DateCreated)}");
            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand(PriceAndChargingEntity model)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.OverstayPriceConfiguration, " SET "));


            sql.Append(string.Concat(" ", nameof(model.StoragePrice), " = ",
                    "@", nameof(model.StoragePrice)));

            sql.Append(string.Concat(", ", nameof(model.MultiAccessStoragePrice), " = ",
             "@", nameof(model.MultiAccessStoragePrice)));

            sql.Append(string.Concat(", ", nameof(model.OverstayCharge), " = ",
                     "@", nameof(model.OverstayCharge)));

            sql.Append(string.Concat(", ", nameof(model.LockerTypeId), " = ",

                     "@", nameof(model.LockerTypeId)));
            sql.Append(string.Concat(", ", nameof(model.LocationId), " = ",
                    "@", nameof(model.LocationId)));

            sql.Append(string.Concat(", ", nameof(model.PricingMatrixId), " = ",
                   "@", nameof(model.PricingMatrixId)));

            sql.Append(string.Concat(", ", nameof(model.DateModified), " = ",
                    "@", nameof(model.DateModified)));


            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();

        }

    }
}
