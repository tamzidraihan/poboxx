using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.User
{
    public class UserSubscriptionBillingRepository : GenericRepositoryBase<UserSubscriptionBillingEntity, UserSubscriptionBillingRepository>, IUserSubscriptionBillingRepository
    {
        public UserSubscriptionBillingRepository(IDatabaseHelper databaseHelper, ILogger<UserSubscriptionBillingRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<UserSubscriptionBillingEntity>> GetUserSubscriptionBillingBySubscriptionId(int userSubscriptionId)
        {
            var sql = BuildGetCommand();
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserSubscriptionBillingEntity.Id)), userSubscriptionId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<UserSubscriptionBillingEntity>(sql, p)).ToList();
            }
        }
        string BuildGetCommand()
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.UserSubscriptionBilling));

            sql.Append(string.Concat(" WHERE ", nameof(UserSubscriptionBillingEntity.Id), " = ", "@", nameof(UserSubscriptionBillingEntity.Id)));

            return sql.ToString();
        }


        public async Task<int> Save(UserSubscriptionBillingEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.UserSubscriptionId)), model.UserSubscriptionId);
            p.Add(string.Concat("@", nameof(model.PaidAmount)), model.PaidAmount);
            p.Add(string.Concat("@", nameof(model.PaymentDate)), model.PaymentDate);

            string sql;
            if (model.Id == 0)
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
                    if (!string.IsNullOrEmpty(e.Message) && e.Message.Contains("foreign key"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.InvalidForeignId;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }

        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.UserSubscriptionBilling));

            sql.Append(" (");
            sql.Append(nameof(UserSubscriptionBillingEntity.UserSubscriptionId));
            sql.Append($", {nameof(UserSubscriptionBillingEntity.PaidAmount)}");
            sql.Append($", {nameof(UserSubscriptionBillingEntity.PaymentDate)}");
            sql.Append($", {nameof(UserSubscriptionBillingEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(UserSubscriptionBillingEntity.UserSubscriptionId)}");
            sql.Append($", @{nameof(UserSubscriptionBillingEntity.PaidAmount)}");
            sql.Append($", @{nameof(UserSubscriptionBillingEntity.PaymentDate)}");
            sql.Append($", @{nameof(UserSubscriptionBillingEntity.DateCreated)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.UserSubscriptionBilling, " SET "));

            sql.Append(string.Concat(" ", nameof(UserSubscriptionBillingEntity.UserSubscriptionId), " = ",
                    "@", nameof(UserSubscriptionBillingEntity.UserSubscriptionId)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionBillingEntity.PaidAmount), " = ",
                     "@", nameof(UserSubscriptionBillingEntity.PaidAmount)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionBillingEntity.PaymentDate), " = ",
                    "@", nameof(UserSubscriptionBillingEntity.PaymentDate)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionBillingEntity.DateModified), " = ",
                    "@", nameof(UserSubscriptionBillingEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(UserSubscriptionBillingEntity.Id), " = ", "@", nameof(UserSubscriptionBillingEntity.Id)));

            return sql.ToString();

        }
        public async Task<int> Delete(int id)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserSubscriptionBillingEntity.Id)), id);
            string sql = BuildDeleteCommand();

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
        string BuildDeleteCommand()
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.UserSubscriptionBilling));

            sql.Append(string.Concat(" WHERE ", nameof(UserSubscriptionBillingEntity.Id), " = ", "@", nameof(UserSubscriptionBillingEntity.Id)));

            return sql.ToString();

        }
    }
}
