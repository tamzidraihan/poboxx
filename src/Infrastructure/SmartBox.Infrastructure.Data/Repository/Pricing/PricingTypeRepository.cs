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
    public class PricingTypeRepository : GenericRepositoryBase<PricingTypeEntity, PricingTypeRepository>, IPricingTypeRepository
    {
        public PricingTypeRepository(IDatabaseHelper databaseHelper, ILogger<PricingTypeRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<PricingTypeEntity>> Get()
        {
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<PricingTypeEntity>(BuildGetCommand())).ToList();
            }
        }

        public async Task<int> Save(PricingTypeEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.Description)), model.Description);
            p.Add(string.Concat("@", nameof(model.Name)), model.Name);

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
        string BuildGetCommand()
        {
            return string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.PricingType);
        }

        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.PricingType));

            sql.Append(" (");
            sql.Append(nameof(PricingTypeEntity.Name));
            sql.Append($", {nameof(PricingTypeEntity.Description)}");
            sql.Append($", {nameof(PricingTypeEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(PricingTypeEntity.Name)}");
            sql.Append($", @{nameof(PricingTypeEntity.Description)}");
            sql.Append($", @{nameof(PricingTypeEntity.DateCreated)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand(PricingTypeEntity model)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.PricingType, " SET "));


            sql.Append(string.Concat(" ", nameof(model.Description), " = ",
                    "@", nameof(model.Description)));

            sql.Append(string.Concat(", ", nameof(model.Name), " = ",
                     "@", nameof(model.Name)));

            sql.Append(string.Concat(", ", nameof(model.DateModified), " = ",
                    "@", nameof(model.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();

        }
        public async Task<int> Delete(int id)
        {
            PricingTypeEntity entity = new PricingTypeEntity { Id = id };
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
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.PriceTypeDeleteConstraintsError;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildDeleteCommand(PricingTypeEntity model)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.PricingType));

            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();

        }
    }
}
