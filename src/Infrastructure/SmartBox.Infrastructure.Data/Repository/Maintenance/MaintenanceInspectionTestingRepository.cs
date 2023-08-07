using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Maintenance;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Maintenance
{
    public class MaintenanceInspectionTestingRepository : GenericRepositoryBase<MaintenanceInspectionTestingEntity, MaintenanceInspectionTestingRepository>, IMaintenanceInspectionTestingRepository
    {
        public MaintenanceInspectionTestingRepository(IDatabaseHelper databaseHelper, ILogger<MaintenanceInspectionTestingRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<MaintenanceInspectionTestingEntity>> Get(DateTime? fromDate, DateTime? toDate, int? companyId, int? cabinetLocationId)
        {
            var sql = BuildGetCommand(fromDate, toDate, companyId, cabinetLocationId);
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(MaintenanceInspectionTestingEntity.CompanyId)), companyId);
            p.Add(string.Concat("@", nameof(MaintenanceInspectionTestingEntity.CabinetLocationId)), cabinetLocationId);
            p.Add(string.Concat("@", nameof(MaintenanceInspectionTestingEntity.FromDate)), fromDate.HasValue ? fromDate.Value.Date : fromDate);
            p.Add(string.Concat("@", nameof(MaintenanceInspectionTestingEntity.ToDate)), toDate.HasValue ? toDate.Value.Date : toDate);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<MaintenanceInspectionTestingEntity>(sql, p)).ToList();
            }
        }
        string BuildGetCommand(DateTime? fromDate, DateTime? toDate, int? companyId, int? cabinetLocationId)
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.MaintenanceInspectionTesting));
            if (fromDate.HasValue || toDate.HasValue || companyId.HasValue || cabinetLocationId.HasValue)
            {
                sql.Append(" WHERE ");
                bool multipleFilters = false;
                if (companyId.HasValue)
                {
                    sql.Append(string.Concat(nameof(MaintenanceInspectionTestingEntity.CompanyId), " = ", "@", nameof(MaintenanceInspectionTestingEntity.CompanyId)));
                    multipleFilters = true;
                }
                if (cabinetLocationId.HasValue)
                {
                    if (multipleFilters)
                        sql.Append(" AND ");
                    sql.Append(string.Concat(nameof(MaintenanceInspectionTestingEntity.CabinetLocationId), " = ", "@", nameof(MaintenanceInspectionTestingEntity.CabinetLocationId)));
                    multipleFilters = true;
                }
                if (fromDate.HasValue)
                {
                    if (multipleFilters)
                        sql.Append(" AND ");
                    sql.Append(string.Concat("CAST(", nameof(MaintenanceInspectionTestingEntity.DateCreated), " AS DATE) ", " >= ", "@", nameof(MaintenanceInspectionTestingEntity.FromDate)));
                    multipleFilters = true;
                }
                if (toDate.HasValue)
                {
                    if (multipleFilters)
                        sql.Append(" AND ");
                    sql.Append(string.Concat("CAST(", nameof(MaintenanceInspectionTestingEntity.DateCreated), " AS DATE) ", " <= ", "@", nameof(MaintenanceInspectionTestingEntity.ToDate)));
                }
            }
            return sql.ToString();

        }

        public async Task<int> Save(MaintenanceInspectionTestingEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.TypeId)), model.TypeId);
            p.Add(string.Concat("@", nameof(model.LockerTypeId)), model.LockerTypeId);
            p.Add(string.Concat("@", nameof(model.MaintenanceReasonTypeId)), model.MaintenanceReasonTypeId);
            p.Add(string.Concat("@", nameof(model.CabinetId)), model.CabinetId);
            p.Add(string.Concat("@", nameof(model.CompanyId)), model.CompanyId);
            p.Add(string.Concat("@", nameof(model.LockerDetailId)), model.LockerDetailId);
            p.Add(string.Concat("@", nameof(model.LockerNumber)), model.LockerNumber);
            p.Add(string.Concat("@", nameof(model.Message)), model.Message);
            p.Add(string.Concat("@", nameof(model.Status)), model.Status);

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

        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.MaintenanceInspectionTesting));

            sql.Append(" (");
            sql.Append(nameof(MaintenanceInspectionTestingEntity.TypeId));
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.MaintenanceReasonTypeId)}");
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.CabinetId)}");
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.CompanyId)}");
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.LockerDetailId)}");
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.LockerNumber)}");
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.LockerTypeId)}");
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.Message)}");
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.Status)}");
            sql.Append($", {nameof(MaintenanceInspectionTestingEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(MaintenanceInspectionTestingEntity.TypeId)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.MaintenanceReasonTypeId)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.CabinetId)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.CompanyId)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.LockerDetailId)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.LockerNumber)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.LockerTypeId)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.Message)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.Status)}");
            sql.Append($", @{nameof(MaintenanceInspectionTestingEntity.DateCreated)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand(MaintenanceInspectionTestingEntity model)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.MaintenanceInspectionTesting, " SET "));

            sql.Append(string.Concat(" ", nameof(model.TypeId), " = ",
                    "@", nameof(model.TypeId)));
            sql.Append(string.Concat(", ", nameof(model.LockerNumber), " = ",
                     "@", nameof(model.LockerNumber)));
            sql.Append(string.Concat(", ", nameof(model.MaintenanceReasonTypeId), " = ",
                    "@", nameof(model.MaintenanceReasonTypeId)));
            sql.Append(string.Concat(", ", nameof(model.CabinetId), " = ",
                    "@", nameof(model.CabinetId)));
            sql.Append(string.Concat(", ", nameof(model.CompanyId), " = ",
                    "@", nameof(model.CompanyId)));
            sql.Append(string.Concat(", ", nameof(model.LockerDetailId), " = ",
                  "@", nameof(model.LockerDetailId)));
            sql.Append(string.Concat(", ", nameof(model.LockerTypeId), " = ",
                  "@", nameof(model.LockerTypeId)));
            sql.Append(string.Concat(", ", nameof(model.Message), " = ",
                "@", nameof(model.Message)));
            sql.Append(string.Concat(", ", nameof(model.Status), " = ",
               "@", nameof(model.Status)));
            sql.Append(string.Concat(", ", nameof(model.DateModified), " = ",
                "@", nameof(model.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();

        }
        public async Task<int> Delete(int id)
        {
            MaintenanceInspectionTestingEntity entity = new MaintenanceInspectionTestingEntity { Id = id };
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
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildDeleteCommand(MaintenanceInspectionTestingEntity model)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.MaintenanceInspectionTesting));

            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();

        }
    }
}
