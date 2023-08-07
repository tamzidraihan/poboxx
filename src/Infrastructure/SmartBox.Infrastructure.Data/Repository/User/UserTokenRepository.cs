using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Notification;
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
    public class UserTokenRepository : GenericRepositoryBase<UserTokenEntity, UserTokenRepository>, IUserTokenRepository
    {
        public UserTokenRepository(IDatabaseHelper databaseHelper, ILogger<UserTokenRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<UserTokenEntity>> GetUserTokenByUserId(int userId, string deviceDecription = "", DeviceType? deviceType = null)
        {
            if (userId < 1)
                return null;
            var sql = BuildGetCommandByUserId(deviceDecription, deviceType);
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserTokenEntity.UserId)), userId);
            p.Add(string.Concat("@", nameof(UserTokenEntity.Description)), deviceDecription);
            p.Add(string.Concat("@", nameof(UserTokenEntity.DeviceType)), deviceType);


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<UserTokenEntity>(sql, p)).ToList();
            }
        }

        public async Task<int> Save(UserTokenEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.Token)), model.Token);
            p.Add(string.Concat("@", nameof(model.IsEnable)), model.IsEnable);
            p.Add(string.Concat("@", nameof(model.UserId)), model.UserId);
            p.Add(string.Concat("@", nameof(model.URL)), model.URL);
            p.Add(string.Concat("@", nameof(model.Description)), model.Description);
            p.Add(string.Concat("@", nameof(model.DeviceType)), model.DeviceType);

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
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildGetCommandByUserId(string deviceDecription = "", DeviceType? deviceType = null)
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.UserToken));
            sql.Append(string.Concat(" WHERE ", nameof(UserTokenEntity.UserId), " = ", "@", nameof(UserTokenEntity.UserId)));
            if (!string.IsNullOrEmpty(deviceDecription))
                sql.Append(string.Concat(" AND ", nameof(UserTokenEntity.Description), " = ", "@", nameof(UserTokenEntity.Description)));
            if (deviceType.HasValue)
                sql.Append(string.Concat(" AND ", nameof(UserTokenEntity.DeviceType), " = ", "@", nameof(UserTokenEntity.DeviceType)));
            return sql.ToString();
        }

        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.UserToken));

            sql.Append(" (");
            sql.Append(nameof(UserTokenEntity.UserId));
            sql.Append($", {nameof(UserTokenEntity.Token)}");
            sql.Append($", {nameof(UserTokenEntity.DeviceType)}");
            sql.Append($", {nameof(UserTokenEntity.Description)}");
            sql.Append($", {nameof(UserTokenEntity.URL)}");
            sql.Append($", {nameof(UserTokenEntity.IsEnable)}");
            sql.Append($", {nameof(UserTokenEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(UserTokenEntity.UserId)}");
            sql.Append($", @{nameof(UserTokenEntity.Token)}");
            sql.Append($", @{nameof(UserTokenEntity.DeviceType)}");
            sql.Append($", @{nameof(UserTokenEntity.Description)}");
            sql.Append($", @{nameof(UserTokenEntity.URL)}");
            sql.Append($", @{nameof(UserTokenEntity.IsEnable)}");
            sql.Append($", @{nameof(UserTokenEntity.DateCreated)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.UserToken, " SET "));


            sql.Append(string.Concat(" ", nameof(UserTokenEntity.UserId), " = ",
                    "@", nameof(UserTokenEntity.UserId)));

            sql.Append(string.Concat(", ", nameof(UserTokenEntity.Token), " = ",
                     "@", nameof(UserTokenEntity.Token)));

            sql.Append(string.Concat(", ", nameof(UserTokenEntity.Description), " = ",
                    "@", nameof(UserTokenEntity.Description)));

            sql.Append(string.Concat(", ", nameof(UserTokenEntity.IsEnable), " = ",
                    "@", nameof(UserTokenEntity.IsEnable)));

            sql.Append(string.Concat(", ", nameof(UserTokenEntity.DeviceType), " = ",
                    "@", nameof(UserTokenEntity.DeviceType)));

            sql.Append(string.Concat(", ", nameof(UserTokenEntity.URL), " = ",
                    "@", nameof(UserTokenEntity.URL)));

            sql.Append(string.Concat(", ", nameof(UserTokenEntity.DateModified), " = ",
                    "@", nameof(UserTokenEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(UserTokenEntity.Id), " = ", "@", nameof(UserTokenEntity.Id)));

            return sql.ToString();

        }
        public async Task<int> Delete(int id)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserTokenEntity.Id)), id);
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
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.UserToken));

            sql.Append(string.Concat(" WHERE ", nameof(UserTokenEntity.Id), " = ", "@", nameof(UserTokenEntity.Id)));

            return sql.ToString();

        }
    }
}
