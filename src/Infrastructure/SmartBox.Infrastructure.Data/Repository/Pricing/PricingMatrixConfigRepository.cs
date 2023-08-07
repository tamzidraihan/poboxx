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
    public class PricingMatrixConfigRepository : GenericRepositoryBase<PricingMatrixConfigEntity, PricingMatrixConfigRepository>, IPricingMatrixConfigRepository
    {
        public PricingMatrixConfigRepository(IDatabaseHelper databaseHelper, ILogger<PricingMatrixConfigRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<PricingMatrixConfigEntity>> Get(int? PricingTypeId = null, int? selectedId = null, short? isActive=null)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(PricingMatrixConfigEntity.PricingTypeId)), PricingTypeId);
            p.Add(string.Concat("@", nameof(PricingMatrixConfigEntity.Id)), selectedId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<PricingMatrixConfigEntity>(BuildGetCommand(PricingTypeId, selectedId,isActive))).ToList();
            }
        }

        public async Task<int> Save(PricingMatrixConfigEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.PricingTypeId)), model.PricingTypeId);
            p.Add(string.Concat("@", nameof(model.OverstayPeriod)), model.OverstayPeriod);
            p.Add(string.Concat("@", nameof(model.IsPromoEnabled)), model.IsPromoEnabled);

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
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildGetCommand(int? PricingTypeId = null, int? selectedId = null, short? isActive = null)
        {
            var query = "select t.Name, m.* from pricing_type t, pricing_matrix m where m.pricingtypeid = t.id";
            if (PricingTypeId.HasValue && PricingTypeId.Value > 0)
                query += $" AND t.id={PricingTypeId.Value}";
            if (selectedId.HasValue && selectedId.Value > 0)
                query += $" AND m.id<>{selectedId.Value}";
            if (isActive.HasValue)
                query += $" AND m.isActive = {isActive.Value}";
            return query;
        }

        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.PricingMatrix));

            sql.Append(" (");
            sql.Append(nameof(PricingMatrixConfigEntity.PricingTypeId));
            sql.Append($", {nameof(PricingMatrixConfigEntity.OverstayPeriod)}");
            sql.Append($", {nameof(PricingMatrixConfigEntity.IsPromoEnabled)}");
            sql.Append($", {nameof(PricingMatrixConfigEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(PricingMatrixConfigEntity.PricingTypeId)}");
            sql.Append($", @{nameof(PricingMatrixConfigEntity.OverstayPeriod)}");
            sql.Append($", @{nameof(PricingMatrixConfigEntity.IsPromoEnabled)}");
            sql.Append($", @{nameof(PricingMatrixConfigEntity.DateCreated)}");
            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand(PricingMatrixConfigEntity model)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.PricingMatrix, " SET "));


            sql.Append(string.Concat(" ", nameof(model.PricingTypeId), " = ",
                    "@", nameof(model.PricingTypeId)));

            sql.Append(string.Concat(", ", nameof(model.OverstayPeriod), " = ",
                     "@", nameof(model.OverstayPeriod)));

            sql.Append(string.Concat(", ", nameof(model.IsPromoEnabled), " = ",
                     "@", nameof(model.IsPromoEnabled)));

            sql.Append(string.Concat(", ", nameof(model.DateModified), " = ",
                    "@", nameof(model.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();

        }
        public async Task<int> Delete(int id)
        {
            var entity = new PricingMatrixConfigEntity { Id = id };
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
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.PricingMatrixDeleteConstraints;


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
        string BuildDeleteCommand(PricingMatrixConfigEntity model)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.PricingMatrix));

            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();

        }

        public async Task<int>ActivateDeactivate(int Id, int? isActive)
        {
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
            

                var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.PricingMatrix, " SET "));
                sql.Append($"isActive = {isActive} WHERE Id={Id}");

                try
                {
                    var ret = await conn.ExecuteAsync(sql.ToString());
                    if (ret > 0)
                    {
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                    }
                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;


                    return ret;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
    }
}
