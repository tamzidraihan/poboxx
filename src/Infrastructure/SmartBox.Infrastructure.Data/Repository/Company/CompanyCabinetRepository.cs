using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Company;
using SmartBox.Business.Core.Models.Company;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Company
{
    public class CompanyCabinetRepository : GenericRepositoryBase<CompanyCabinetEntity, CompanyCabinetRepository>, ICompanyCabinetRepository
    {
        public CompanyCabinetRepository(IDatabaseHelper databaseHelper, ILogger<CompanyCabinetRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<CompanyCabinetEntity>> Get(int? companyId = null, int? cabinetId = null, bool? unAssignedOnly = null)
        {
            var sql = BuildGetCommand(companyId, cabinetId, unAssignedOnly);
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.CompanyId)), companyId);
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.CabinetId)), cabinetId);
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<CompanyCabinetEntity>(sql, p)).ToList();
            }
        }
        string BuildGetCommand(int? companyId = null, int? cabinetId = null, bool? unAssignedOnly = null)
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.CompanyAndCabinet));

            if (companyId.HasValue && companyId.Value > 0)
            {
                sql.Append(string.Concat(" WHERE ", nameof(CompanyCabinetEntity.CompanyId), " = ", "@", nameof(CompanyCabinetEntity.CompanyId)));
                if (cabinetId.HasValue && cabinetId.Value > 0)
                    sql.Append(string.Concat(" AND ", nameof(CompanyCabinetEntity.CabinetId), " = ", "@", nameof(CompanyCabinetEntity.CabinetId)));
                if (unAssignedOnly.HasValue)
                {
                    if (unAssignedOnly.Value)
                        sql.Append(string.Concat(" AND ", nameof(CompanyCabinetEntity.IsAssigned), " IS NULL OR ", nameof(CompanyCabinetEntity.IsAssigned), "=FALSE"));
                    else
                        sql.Append(string.Concat(" AND ", nameof(CompanyCabinetEntity.IsAssigned), "=TRUE"));
                }

            }
            else if (cabinetId.HasValue && cabinetId.Value > 0)
            {
                sql.Append(string.Concat(" WHERE ", nameof(CompanyCabinetEntity.CabinetId), " = ", "@", nameof(CompanyCabinetEntity.CabinetId)));
                if (unAssignedOnly.HasValue)
                {
                    if (unAssignedOnly.Value)
                        sql.Append(string.Concat(" AND ", nameof(CompanyCabinetEntity.IsAssigned), " IS NULL OR ", nameof(CompanyCabinetEntity.IsAssigned), "=FALSE"));
                    else
                        sql.Append(string.Concat(" AND ", nameof(CompanyCabinetEntity.IsAssigned), "=TRUE"));
                }

            }
            else if (unAssignedOnly.HasValue)
            {
                if (unAssignedOnly.Value)
                    sql.Append(string.Concat(" WHERE ", nameof(CompanyCabinetEntity.IsAssigned), " IS NULL OR ", nameof(CompanyCabinetEntity.IsAssigned), "=FALSE"));
                else
                    sql.Append(string.Concat(" WHERE ", nameof(CompanyCabinetEntity.IsAssigned), "=TRUE"));

               
            }

            return sql.ToString();
        }
        public async Task SaveBulkRecords(List<int> cabinetIds, int companyId)
        {
            var sql = $"Insert into {GlobalDatabaseConstants.DatabaseTables.CompanyCabinet} ({nameof(CompanyCabinetEntity.CompanyId)},{nameof(CompanyCabinetEntity.CabinetId)},{nameof(CompanyCabinetEntity.IsActive)},{nameof(CompanyCabinetEntity.DateCreated)}) values ";
            var separatorRequired = false;
            foreach (var item in cabinetIds)
            {
                if (separatorRequired)
                    sql += " , ";
                sql += $"({companyId},{item},1,@{nameof(CompanyCabinetEntity.DateCreated)})";
                separatorRequired = true;
            }
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.DateCreated)), DateTime.Now);
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    await conn.ExecuteAsync(sql, p);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);

                }
            }

        }
        public async Task<int> Save(CompanyCabinetEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.CompanyCabinetId)), model.CompanyCabinetId);
            p.Add(string.Concat("@", nameof(model.CompanyId)), model.CompanyId);
            p.Add(string.Concat("@", nameof(model.CabinetId)), model.CabinetId);
            p.Add(string.Concat("@", nameof(model.IsActive)), model.IsActive);

            string sql;
            if (model.CompanyCabinetId == 0)
            {
                p.Add(string.Concat("@", nameof(model.DateCreated)), DateTime.Now);
                sql = BuildInsertCommand();
            }
            else
            {
                p.Add(string.Concat("@", nameof(model.DateModified)), DateTime.Now);
                sql = BuildUpdateCommand();
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

        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.CompanyCabinet));

            sql.Append(" (");
            sql.Append(nameof(CompanyCabinetEntity.CompanyId));
            sql.Append($", {nameof(CompanyCabinetEntity.CabinetId)}");
            sql.Append($", {nameof(CompanyCabinetEntity.IsActive)}");
            sql.Append($", {nameof(CompanyCabinetEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(CompanyCabinetEntity.CompanyId)}");
            sql.Append($", @{nameof(CompanyCabinetEntity.CabinetId)}");
            sql.Append($", @{nameof(CompanyCabinetEntity.IsActive)}");
            sql.Append($", @{nameof(CompanyCabinetEntity.DateCreated)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.CompanyCabinet, " SET "));


            sql.Append(string.Concat(" ", nameof(CompanyCabinetEntity.CompanyId), " = ",
                    "@", nameof(CompanyCabinetEntity.CompanyId)));

            sql.Append(string.Concat(", ", nameof(CompanyCabinetEntity.CabinetId), " = ",
                     "@", nameof(CompanyCabinetEntity.CabinetId)));


            sql.Append(string.Concat(", ", nameof(CompanyCabinetEntity.IsActive), " = ",
                     "@", nameof(CompanyCabinetEntity.IsActive)));

            sql.Append(string.Concat(", ", nameof(CompanyCabinetEntity.DateModified), " = ",
                    "@", nameof(CompanyCabinetEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(CompanyCabinetEntity.CompanyCabinetId), " = ", "@", nameof(CompanyCabinetEntity.CompanyCabinetId)));

            return sql.ToString();

        }
        public async Task<int> AssignCompanyCabinets(AssignCompanyCabinetModel model)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.CompanyId)), model.CompanyId);
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.CabinetId)), model.CabinetId);
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.IsAssigned)), true);
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.DateModified)), DateTime.Now);
            string sql;
            sql = BuildUpdateCommandForAssignCompanyCabinets();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
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

        public async Task<int> UnassignCompanyCabinets(AssignCompanyCabinetModel model)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.CompanyId)), model.CompanyId);
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.CabinetId)), model.CabinetId);
  
            p.Add(string.Concat("@", nameof(CompanyCabinetEntity.DateModified)), DateTime.Now);
            string sql;
            sql = BuildUpdateCommandForUnAssignCompanyCabinets();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
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
        string BuildUpdateCommandForAssignCompanyCabinets()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.CompanyCabinet, " SET "));


            sql.Append(string.Concat(" ", nameof(CompanyCabinetEntity.IsAssigned), " = ",
                    "@", nameof(CompanyCabinetEntity.IsAssigned)));

            sql.Append(string.Concat(", ", nameof(CompanyCabinetEntity.DateModified), " = ",
                    "@", nameof(CompanyCabinetEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(CompanyCabinetEntity.CompanyId), " = ", "@", nameof(CompanyCabinetEntity.CompanyId)));

            sql.Append(string.Concat(" AND ", nameof(CompanyCabinetEntity.CabinetId), " = ", "@", nameof(CompanyCabinetEntity.CabinetId)));

            return sql.ToString();

        }
        string BuildUpdateCommandForUnAssignCompanyCabinets()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.CompanyCabinet, " SET "));


            sql.Append(string.Concat(" ", nameof(CompanyCabinetEntity.IsAssigned), " = ",
                    "NULL", ""));

            sql.Append(string.Concat(", ", nameof(CompanyCabinetEntity.DateModified), " = ",
                    "@", nameof(CompanyCabinetEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(CompanyCabinetEntity.CompanyId), " = ", "@", nameof(CompanyCabinetEntity.CompanyId)));

            sql.Append(string.Concat(" AND ", nameof(CompanyCabinetEntity.CabinetId), " = ", "@", nameof(CompanyCabinetEntity.CabinetId)));

            return sql.ToString();

        }

    }
}
