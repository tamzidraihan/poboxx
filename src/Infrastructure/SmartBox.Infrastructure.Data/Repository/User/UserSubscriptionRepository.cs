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
    public class UserSubscriptionRepository : GenericRepositoryBase<UserSubscriptionEntity, UserSubscriptionRepository>, IUserSubscriptionRepository
    {
        public UserSubscriptionRepository(IDatabaseHelper databaseHelper, ILogger<UserSubscriptionRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<UserSubscriptionEntity>> GetUserSubscription(string userKeyId, int? lockerDetailId = null, DateTime? storagePeriodEndDate = null)
        {
            if (string.IsNullOrEmpty(userKeyId))
                return null;
            var sql = BuildGetCommandForUserSubscription(lockerDetailId, storagePeriodEndDate);
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserSubscriptionEntity.UserKeyId)), userKeyId);
            p.Add(string.Concat("@", nameof(UserSubscriptionEntity.LockerDetailId)), lockerDetailId);
            p.Add(string.Concat("@", nameof(UserSubscriptionEntity.ExpiryDate)), storagePeriodEndDate);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<UserSubscriptionEntity>(sql, p)).ToList();
            }
        }

        public async Task<Tuple<int, int>> Save(UserSubscriptionEntity model, string loginUserKeyId = null)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.UserKeyId)), model.UserKeyId);
            p.Add(string.Concat("@", nameof(model.Price)), model.Price);
            p.Add(string.Concat("@", nameof(model.LockerTypeId)), model.LockerTypeId);
            p.Add(string.Concat("@", nameof(model.LockerDetailId)), model.LockerDetailId);
            p.Add(string.Concat("@", nameof(model.CabinetLocationId)), model.CabinetLocationId);
            p.Add(string.Concat("@", nameof(model.ExpiryDate)), model.ExpiryDate);
            p.Add(string.Concat("@", nameof(model.NextBillingDate)), model.NextBillingDate);
            p.Add(string.Concat("@", nameof(model.LockerTransactionsId)), model.LockerTransactionsId);

            if (!string.IsNullOrEmpty(loginUserKeyId))
                p.Add("@LoginUserKeyId", loginUserKeyId);


            Tuple<int, int> response;
            string sql;
            if (model.Id == 0)
            {
                p.Add(string.Concat("@", nameof(model.DateCreated)), DateTime.Now);
                sql = BuildInsertCommand();
            }
            else
            {
                p.Add(string.Concat("@", nameof(model.DateModified)), DateTime.Now);
                sql = BuildUpdateCommand(loginUserKeyId);
                isInsert = false;
            }

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var newId = await conn.ExecuteScalarAsync<int>(sql, p, transaction);
                    if (newId > 0)
                    {
                        if (isInsert)
                            response = new Tuple<int, int>(newId, GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded);
                        else
                        {
                            response = new Tuple<int, int>(model.Id, GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated);
                            isInsert = false;
                        }
                    }
                    else
                        response = new Tuple<int, int>(0, GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    response = new Tuple<int, int>(0, GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError);
                }
                return response;
            }

        }
        string BuildGetCommandForUserSubscription(int? lockerDetailId = null, DateTime? storagePeriodEndDate = null)
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.UserSubscription));


            sql.Append(string.Concat(" WHERE ", nameof(UserSubscriptionEntity.UserKeyId), " = ", "@", nameof(UserSubscriptionEntity.UserKeyId)));

            if (lockerDetailId.HasValue)
                sql.Append(string.Concat(" AND ", nameof(UserSubscriptionEntity.LockerDetailId), " = ", "@", nameof(UserSubscriptionEntity.LockerDetailId)));

            if (storagePeriodEndDate.HasValue)
                sql.Append(string.Concat(" AND ", nameof(UserSubscriptionEntity.ExpiryDate), " >= ", "@", nameof(UserSubscriptionEntity.ExpiryDate)));

            return sql.ToString();
        }

        public async Task<List<UserSubscriptionExpirationEntity>> GetUserSubscriptionExpiration()
        {

            var sql = BuildGetCommandForUserSubscriptionExpiration();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<UserSubscriptionExpirationEntity>(sql)).ToList();
            }
        }
        string BuildGetCommandForUserSubscriptionExpiration()
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.UserSubscriptionExpiration));

            sql.Append(string.Concat($" WHERE Date(Now()+interval 7 day)>Date({nameof(UserSubscriptionEntity.ExpiryDate)})"));

            return sql.ToString();
        }
        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.UserSubscription));

            sql.Append(" (");
            sql.Append(nameof(UserSubscriptionEntity.UserKeyId));
            sql.Append($", {nameof(UserSubscriptionEntity.Price)}");
            sql.Append($", {nameof(UserSubscriptionEntity.CabinetLocationId)}");
            sql.Append($", {nameof(UserSubscriptionEntity.ExpiryDate)}");
            sql.Append($", {nameof(UserSubscriptionEntity.LockerDetailId)}");
            sql.Append($", {nameof(UserSubscriptionEntity.LockerTypeId)}");
            sql.Append($", {nameof(UserSubscriptionEntity.NextBillingDate)}");
            sql.Append($", {nameof(UserSubscriptionEntity.DateCreated)}");
            sql.Append($", {nameof(UserSubscriptionEntity.LockerTransactionsId)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(UserSubscriptionEntity.UserKeyId)}");
            sql.Append($", @{nameof(UserSubscriptionEntity.Price)}");
            sql.Append($", @{nameof(UserSubscriptionEntity.CabinetLocationId)}");
            sql.Append($", @{nameof(UserSubscriptionEntity.ExpiryDate)}");
            sql.Append($", @{nameof(UserSubscriptionEntity.LockerDetailId)}");
            sql.Append($", @{nameof(UserSubscriptionEntity.LockerTypeId)}");
            sql.Append($", @{nameof(UserSubscriptionEntity.NextBillingDate)}");
            sql.Append($", @{nameof(UserSubscriptionEntity.DateCreated)}");
            sql.Append($", @{nameof(UserSubscriptionEntity.LockerTransactionsId)}");

            sql.Append("); SELECT LAST_INSERT_ID();");

            return sql.ToString();
        }
        string BuildUpdateCommand(string userKeyId = null)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.UserSubscription, " SET "));


            sql.Append(string.Concat(" ", nameof(UserSubscriptionEntity.Price), " = ",
                    "@", nameof(UserSubscriptionEntity.Price)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionEntity.UserKeyId), " = ",
                     "@", nameof(UserSubscriptionEntity.UserKeyId)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionEntity.LockerDetailId), " = ",
                    "@", nameof(UserSubscriptionEntity.LockerDetailId)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionEntity.CabinetLocationId), " = ",
                    "@", nameof(UserSubscriptionEntity.CabinetLocationId)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionEntity.ExpiryDate), " = ",
                    "@", nameof(UserSubscriptionEntity.ExpiryDate)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionEntity.LockerTypeId), " = ",
                    "@", nameof(UserSubscriptionEntity.LockerTypeId)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionEntity.NextBillingDate), " = ",
                   "@", nameof(UserSubscriptionEntity.NextBillingDate)));

            sql.Append(string.Concat(", ", nameof(UserSubscriptionEntity.DateModified), " = ",
                    "@", nameof(UserSubscriptionEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(UserSubscriptionEntity.Id), " = ", "@", nameof(UserSubscriptionEntity.Id)));
            if (!string.IsNullOrEmpty(userKeyId))
                sql.Append(string.Concat(" And ", nameof(UserSubscriptionEntity.UserKeyId), " = ", "@LoginUserKeyId"));
            return sql.ToString();

        }
        public async Task<int> Delete(int id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(UserSubscriptionEntity.Id)), id);
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
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.UserSubscription));

            sql.Append(string.Concat(" WHERE ", nameof(UserSubscriptionEntity.Id), " = ", "@", nameof(UserSubscriptionEntity.Id)));

            return sql.ToString();

        }
        public async Task<UserSubscriptionEntity> GetById(int id)
        {
            var sql = BuildGetById();
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserSubscriptionEntity.Id)), id);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<UserSubscriptionEntity>(sql, p)).FirstOrDefault();
            }
        }
        string BuildGetById()
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.UserSubscription));
            sql.Append(string.Concat(" WHERE ", nameof(UserSubscriptionEntity.Id), " = ", "@", nameof(UserSubscriptionEntity.Id)));

            return sql.ToString();
        }
    }
}
