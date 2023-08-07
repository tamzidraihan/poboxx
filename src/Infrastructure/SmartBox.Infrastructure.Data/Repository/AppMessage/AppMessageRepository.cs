using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.AppMessage;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Entities.Permission;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;


namespace SmartBox.Infrastructure.Data.Repository.AppMessage
{
    public class AppMessageRepository : GenericRepositoryBase<ApplicationMessageEntity, AppMessageRepository>, IAppMessageRepository
    {

        public AppMessageRepository(IDatabaseHelper databaseHelper, ILogger<AppMessageRepository> logger) : base(databaseHelper, logger)
        {
        }
        string QueryBuilder()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.ApplicationMessage}");
            query.Append($" WHERE {nameof(MessageEntity.ApplicationMessageId)} = {GlobalDatabaseConstants.QueryParameters.ApplicationMessageId}");

            return query.ToString();
        }
        public async Task<ApplicationMessageEntity> GetApplicationMessage(int applicationMessageId)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.ApplicationMessageId, applicationMessageId);

            var sql = QueryBuilder();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<ApplicationMessageEntity>(sql, p);

      

                return dbModel.FirstOrDefault();
            }
        }


        public async Task<int> Create(ApplicationMessageEntity appMessage)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(ApplicationMessageEntity.ApplicationMessageId)), appMessage.ApplicationMessageId);
            p.Add(string.Concat("@", nameof(ApplicationMessageEntity.Message)), appMessage.Message);
            p.Add(string.Concat("@", nameof(ApplicationMessageEntity.Description)), appMessage.Description);
            p.Add(string.Concat("@", nameof(ApplicationMessageEntity.DateCreated)), DateTime.Now);

            sql = BuildInsertScript();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
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

        public async Task<int> Update(ApplicationMessageEntity appMessage)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(ApplicationMessageEntity.ApplicationMessageId)), appMessage.ApplicationMessageId);
            p.Add(string.Concat("@", nameof(ApplicationMessageEntity.Message)), appMessage.Message);
            p.Add(string.Concat("@", nameof(ApplicationMessageEntity.Description)), appMessage.Description);
            p.Add(string.Concat("@", nameof(ApplicationMessageEntity.DateModified)), DateTime.Now);

            sql = BuildUpdateScript(appMessage);
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
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
        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.ApplicationMessage));
            sql.Append(" (");
            sql.Append(nameof(ApplicationMessageEntity.ApplicationMessageId));
            sql.Append($", {nameof(ApplicationMessageEntity.Message)}");
            sql.Append($", {nameof(ApplicationMessageEntity.Description)}");
            sql.Append($", {nameof(ApplicationMessageEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($" @{nameof(ApplicationMessageEntity.ApplicationMessageId)}");
            sql.Append($", @{nameof(ApplicationMessageEntity.Message)}");
            sql.Append($", @{nameof(ApplicationMessageEntity.Description)}");
            sql.Append($", @{nameof(ApplicationMessageEntity.DateCreated)}");


            sql.Append(")");

            return sql.ToString();


        }

        string BuildUpdateScript(ApplicationMessageEntity appMessage)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.ApplicationMessage} SET ");

            sql.Append(string.Concat(nameof(appMessage.Message), " = ",
                   "@", nameof(appMessage.Message)));
            sql.Append(string.Concat(", ", nameof(appMessage.Description), " = ",
                    "@", nameof(appMessage.Description)));
            sql.Append(string.Concat(", ", nameof(appMessage.DateCreated), " = ",
                    "@", nameof(appMessage.DateCreated)));

            sql.Append(string.Concat(" WHERE ", nameof(appMessage.ApplicationMessageId), " = ", "@", nameof(appMessage.ApplicationMessageId)));

            return sql.ToString();


        }


        public async Task<int> DeleteApplicationMessage(int Id)
        {
            ApplicationMessageEntity entity = new ApplicationMessageEntity { ApplicationMessageId = Id };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.ApplicationMessageId)), Id);
            string sql = BuildDeleteScript(entity);

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


        string BuildDeleteScript(ApplicationMessageEntity appMessage)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.ApplicationMessage));

            sql.Append(string.Concat(" WHERE ", nameof(appMessage.ApplicationMessageId), " = ", "@", nameof(appMessage.ApplicationMessageId)));

            return sql.ToString();


        }
        string BuildGetAllScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.ApplicationMessage}");
           

            return sql.ToString();



        }
        public async Task<List<ApplicationMessageEntity>> GetAll()
        {
            var p = new DynamicParameters();


            var sql = BuildGetAllScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<ApplicationMessageEntity>(sql, p);

                return dbModel.ToList();
            }
        }
    }
}
